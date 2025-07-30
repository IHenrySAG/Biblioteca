

let libro
let listaLibro

let autoresSeleccionados=[]
let listaAutores

let listaBibliografias = []

const formBuscarAutor = document.getElementById('frmBuscarAutor')
formBuscarAutor.addEventListener('submit', bucarAutorEvent)

const formCrearPrestamo = document.getElementById('formCrearPrestamo')
formCrearPrestamo.addEventListener('submit', function(evt) {
    evt.preventDefault()

    if (!libro) {
        alert('Debe seleccionar un Libro')
        return
    }

    formCrearPrestamo.submit()
})

$('#frmBuscarEditora').submit(function (evt) {
    evt.preventDefault()
    $('#btnBuscarEditora').trigger('click')
})


async function buscarEditoraClick() {
    var filtro = $('input[name="filtroEditora"]').val()

    listaLibro = await buscarEditorial(filtro)
    $('#tbody-Editoras').html(
        listaLibro.map(editora =>
            `<tr>
            <td>
                ${editora.codigoEditora}
            </td>
            <td>
                ${editora.nombreEditora}
            </td>
            <td>
                ${editora.descripcion}
            </td>
            <td>
                <button class="btn btn-primary text-white" onclick="seleccionarEditorial(${editora.codigoEditora})">Seleccionar</button>
            </td>
        </tr>`
        ))
}

function seleccionarEditorial(codigoEditorial){
    libro = listaLibro.find(x => x.codigoEditora === codigoEditorial)

    $('#editorial-container').html(
        `
        <div class="row align-items-end">
                    <div class="col-10">
                        <div class="form-group mb-2">
                            <label class="control-label">Editorial</label>
                            <input type="hidden" name="CodigoEditora" value="${libro.codigoEditora}" />
                            <input class="form-control" type="text" value="${libro.nombreEditora}" readonly />
                        </div>
                    </div>
                    <div class="col-2">
                            <div class="form-group mb-2">
                                <button class="btn btn-outline-primary" type="button" data-bs-toggle="modal" data-bs-target="#modalSeleccionarEditora">Cambiar</button>
                        </div>
                    </div>
                </div>
        `
    )

    $('#modalSeleccionarEditora').modal('toggle')
}

async function bucarAutorEvent (evt) {
    evt.preventDefault()
    var filtro = $('input[name="filtroAutor"]').val()

    listaAutores = await buscarAutor(filtro)
    $('#tbody-Autores').html(
        listaAutores.map(autor =>
            `<tr>
            <td>
                ${autor.codigoAutor}
            </td>
            <td>
                ${autor.nombreAutor}
            </td>
            <td>
                ${autor.paisOrigen}
            </td>
            <td>
                ${autor.idioma.nombreIdioma}
            </td>
            <td>
                <button class="btn btn-primary text-white" onclick="agregarAutor(${autor.codigoAutor})">Agregar</button>
            </td>
        </tr>`
        ))
}

function agregarAutor(codigoAutor) {
    const autorSeleccionado = listaAutores.find(x => x.codigoAutor === codigoAutor)

    if(!autoresSeleccionados.includes(autorSeleccionado))
        autoresSeleccionados.push(autorSeleccionado)

    cargarListaAutores()

    $('#modalAgregarAutor').modal('toggle')
}

function eliminarAutor(codigoAutor) {
    const indexAutor = autoresSeleccionados.findIndex(x => x.codigoAutor === codigoAutor)
    autoresSeleccionados.splice(indexAutor, 1)
    cargarListaAutores()
}

function cargarListaAutores() {
    $('#autores-container').html(
        `<label class="control-label">Nombre Autor, Idioma</label>
        ${autoresSeleccionados.map(autor => `<div class="input-group mb-2">
            <input type="hidden" name="autores" value="${autor.codigoAutor}" />
            <input type="text" aria-label="Autor" class="form-control" value="${autor.nombreAutor}" readonly>
            <input type="text" aria-label="Pais" class="form-control" value="${autor.paisOrigen}" readonly>
            <button class="btn btn-outline-danger" type="button" onclick="eliminarAutor(${autor.codigoAutor})">Eliminar</button>
        </div>`).join('')}
        `
    )
}

const formAgregarBibliografia = document.getElementById('frmAgregarBibliografia')
formAgregarBibliografia.addEventListener('submit', function(evt) {
    evt.preventDefault()

    const nombreBibliografia = $('#txtNombreBibliografia').val()
    const descripcion = $('#txtDescripcionBibliografia').val()

    if (!nombreBibliografia) {
        alert('La bibliografia debe tener al menos un nombre')
        return
    }

    listaBibliografias.push({
        nombreBibliografia,
        descripcion
    })

    cargarListaBibliografias()

    $('#modalAgregarBibliografia').modal('toggle')
})

function cargarListaBibliografias() {
    $('#bibliografias-container').html(listaBibliografias.map((bibliografia, index) =>
        `
        <div class="mb-3">
            <div class="row mb-0">
                <div class="col-10 mb-0">
                    <div class="form-group mb-2">
                        <label class="control-label">Nombre Bibliografia</label>
                        <input class="form-control" type="text" name="bibliografias[${index}].NombreBibliografia" value="${bibliografia.nombreBibliografia}" readonly />
                    </div>
                </div>
                <div class="col-2"> 
                        <div class="form-group mb-2">
                            <button class="btn btn-outline-primary" type="button" onclick="eliminarBibliografia(${index})">Eliminar</button>
                    </div>
                </div>
            </div>
            <div class="form-floating mb-2">
                <textarea id="txtDescBibliografia${index}" class="form-control" name="bibliografias[${index}].Descripcion"  placeholder="Descripcion de la bibliografia">${bibliografia.descripcion}</textarea>
                <label for="txtDescBibliografia${index}">Descripcion</label>
            </div>
        </div>
        `
    ))
}

function eliminarBibliografia(indexBibliografia) {
    listaBibliografias.splice(indexBibliografia, 1)
    cargarListaBibliografias()
}

async function buscarEditorial(filtro) {
    const result = await fetch(`/Editoras/BuscarEditoraJson?filtro=${filtro}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })

    if (!result.ok) {
        alert('Error al buscar la editorial')
        console.error('Error:', result.statusText)
        return [];
    }

    return await result.json()
}

async function buscarAutor(filtro) {
    const result = await fetch(`/Autores/BuscarAutorJson?filtro=${filtro}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })

    console.log({result})

    if (!result.ok) {
        alert('Error al buscar el autor')
        console.error('Error:', result.statusText)
        return [];
    }

    return await result.json()
}