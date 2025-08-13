

let libro
let listaLibros

let estudiante

const formSeleccionarEstudiante = document.getElementById('frmBuscarEstudiante')
formSeleccionarEstudiante.addEventListener('submit', async function (evt) {
    evt.preventDefault()

    const filtro = $('input[name="filtroEstudiante"]').val()

    if (!filtro.trim()) {
        return
    }

    estudiante = await buscarEstudiante(filtro);

    if (!estudiante) {
        const tipoDocumento = $('#slcTipoDocumentoEstudiante').val();
        const documento = tipoDocumento == 1 ? 'carnet' : 'cedula';
        $('#buscar-estudiante-container').html(`
        <hr />
        <h5 class="text-danger">No se encontro ningun estudiante con ${documento} ${filtro}</h5>
        `)
        return
    }

    $('#buscar-estudiante-container').html(
        `<hr />
        <dl class="row">
            <dt class="col-sm-3">Número Carnet</dt>
            <dd class="col-sm-9">
                ${estudiante.numeroCarnet}
            </dd>
            <dt class="col-sm-3">Cédula</dt>
            <dd class="col-sm-9">
                ${estudiante.cedula}
            </dd>
            <dt class="col-sm-3">Nombre</dt>
            <dd class="col-sm-9">
                ${estudiante.nombre}
            </dd>
            <dt class="col-sm-3">Apellido</dt>
            <dd class="col-sm-9">
                ${estudiante.apellido}
            </dd>
            ${(estudiante.puedeTomarPrestamos
            ? `<div class="row">
                    <div class="col-2">
                        <button class="btn btn-outline-primary" onclick="seleccionarEstudiante()">Seleccionar</button>
                    </div>
                </div>`
            : `<h5 class="col text-danger">${estudiante.errorPrestamo}</h5>`)}
        </dl>`
        )
})

function seleccionarEstudiante() {
    $('#estudiante-container').html(
        `
        <h4>Estudiante seleccionado</h4>
        <hr />
        <dl class="row">
            <input name="CodigoEstudiante" type="hidden" value="${estudiante.codigoEstudiante}">
            <dt class="col-sm-3">Número Carnet</dt>
            <dd class="col-sm-9">
                ${estudiante.numeroCarnet}
            </dd>
            <dt class="col-sm-3">Cédula</dt>
            <dd class="col-sm-9">
                ${estudiante.cedula}
            </dd>
            <dt class="col-sm-3">Nombre</dt>
            <dd class="col-sm-9">
                ${estudiante.nombre}
            </dd>
            <dt class="col-sm-3">Apellido</dt>
            <dd class="col-sm-9">
                ${estudiante.apellido}
            </dd>
        </dl>
        <div class="form-group mb-2">
            <button class="btn btn-outline-primary" type="button" data-bs-toggle="modal" data-bs-target="#modalSeleccionarEstudiante">Cambiar</button>
        </div>
        `
    )

    $('#modalSeleccionarEstudiante').modal('toggle')
}

const formBuscarLibro = document.getElementById('frmBuscarLibro')
formBuscarLibro.addEventListener('submit', async function(evt) {
    evt.preventDefault()

    const filtro = $('input[name="filtroLibro"]').val()

    if (!filtro.trim()) {
        return
    }

    listaLibros = await buscarLibro(filtro);
    $('#tbody-Libros').html(
        listaLibros.map(libro =>
            `<tr>
            <td>
                ${libro.codigoLibro}
            </td>
            <td>
                ${libro.titulo}
            </td>
            <td>
                ${libro.autores[0]}
            </td>
            <td>
                ${libro.ciencia}
            </td>
            <td>
                ${libro.anioPublicacion}
            </td>
            <td>
                ${(libro.inventario === 0
                ? `<span class='text-danger'>No disponible</span>`
                :`<button class="btn btn-primary text-white" onclick="seleccionarLibro(${libro.codigoLibro})">Seleccionar</button>`)}
            </td>
        </tr>`
        ))
})

function seleccionarLibro(codigoLibro) {
    libro = listaLibros.find(x => x.codigoLibro === codigoLibro)

    $('#libro-container').html(
        `
        <h4>Libro seleccionado</h4>
        <hr />
        <dl class="row">
            <input name="CodigoLibro" type="hidden" value="${libro.codigoLibro}">
            <dt class="col-sm-3">Codigo Libro</dt>
            <dd class="col-sm-9">
                ${libro.codigoLibro}
            </dd>
            <dt class="col-sm-3">Titulo</dt>
            <dd class="col-sm-9">
                ${libro.titulo}
            </dd>
            <dt class="col-sm-3">Editora</dt>
            <dd class="col-sm-9">
                ${libro.editora}
            </dd>
            <dt class="col-sm-3">Ciencia</dt>
            <dd class="col-sm-9">
                ${libro.ciencia}
            </dd>
            <dt class="col-sm-3">Año Publicacion</dt>
            <dd class="col-sm-9">
                ${libro.anioPublicacion}
            </dd>
            <dt class="col-sm-3">Signatura Topografica</dt>
            <dd class="col-sm-9">
                ${libro.signaturaTopografica}
            </dd>
            <dt class="col-sm-3">Isbn</dt>
            <dd class="col-sm-9">
                ${libro.isbn}
            </dd>
            <dt class="col-sm-3">Autor/es</dt>
            <dd class="col-sm-9">
                <ul>
                    ${libro.autores.map(autor => `<li>${autor}</li>`)}
                </ul>
            </dd>
        </dl>
        <div class="form-group mb-2">
            <button class="btn btn-outline-primary" type="button" data-bs-toggle="modal" data-bs-target="#modalSeleccionarLibro">Cambiar</button>
        </div>
        `
    )

    $('#modalSeleccionarLibro').modal('toggle')
}

const formCrearPrestamo = document.getElementById('formCrearPrestamo')
formCrearPrestamo.addEventListener('submit', async function (evt) {
    evt.preventDefault()

    if (!libro) {
        alert('Debe seleccionar un libro')
        return
    }

    if (!estudiante) {
        alert('Debe seleccionar un estudiante')
        return
    }

    formCrearPrestamo.submit()
})

async function buscarEstudiante(documento) {
    const tipoDocumento = $('#slcTipoDocumentoEstudiante').val();

    const endpoint = tipoDocumento == 1 ? 'BuscarEstudianteCarnetJson' : 'BuscarEstudianteCedulaJson';

    const result = await fetch(`/Estudiantes/${endpoint}?documento=${documento}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })   

    return await result.json()
}

async function buscarLibro(filtro) {
    const result = await fetch(`/Libros/BuscarLibroJson?filtro=${filtro}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })

    if (!result.ok) {
        alert('Error al buscar el libro')
        console.error('Error:', result.statusText)
        return [];
    }

    return await result.json()
}