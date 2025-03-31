var despliegue = "/apwArchivoClinico";

function permitirSoloNumeros(event) {
    event.target.value = event.target.value.replace(/[^0-9]/g, ''); 
}

const documentTypes = {
    "1": { "label": "DNI", "length": 8 },
    "2": { "label": "No cuenta con DNI", "length": 0 },
    "3": { "label": "Carné de extranjería", "length": 9 },
    "4": { "label": "CUI para menores de edad", "length": 8 },
    "7": { "label": "Certificado de nacido vivo", "length": { "min": 1, "max": 10 } },
    "8": { "label": "Cédula de identidad", "length": 10 }
};

var desplieguePost = "/apwArchivoClinico";
var moduloHistoriaClinicas = "HistoriaClinicas"; 

var hoy = new Date();
var dia = String(hoy.getDate()).padStart(2, '0');
var mes = String(hoy.getMonth() + 1).padStart(2, '0'); 
var año = hoy.getFullYear();

var paginadoHistoria = { "currentPage": 1, "totalPages": 1, "pageSize": 8 };
var paginadoOrden = { "currentPage": 1, "totalPages": 1, "pageSize": 8 };
var paginadoCuenta = { "currentPage": 1, "totalPages": 1, "pageSize": 8 };
var paginadoSis = { "currentPage": 1, "totalPages": 1, "pageSize": 8 };


function toggleUsuario() {
    var menu = document.getElementById("userMenu");
    menu.style.display = (menu.style.display === "block") ? "none" : "block";
}

function toggleFecha() {
    const fechaInput = document.getElementById('fecha');
    const checkbox = document.getElementById('habilitarFecha');
    if (checkbox.checked) {
        fechaInput.disabled = false;
        const today = new Date();
        const formattedDate = today.toISOString().split('T')[0]; 
        fechaInput.value = formattedDate;
    } else {
        fechaInput.disabled = true;
        /*fechaInput.value = ''; */
    }
}

function cargarVista(vista) {
    switch (vista) {
        case 'HistoriasClinicas':
            paginadoHistoria = { "currentPage": 1, "totalPages": 1, "pageSize": 8 };           
            break;
        case 'AuditoriaOrdenes':
            paginadoOrden = { "currentPage": 1, "totalPages": 1, "pageSize": 8 };
            break;
        case 'AuditoriaCuentas':
            paginadoCuenta = { "currentPage": 1, "totalPages": 1, "pageSize": 8 };
            break;
        case 'AuditoriaFUA':
            paginadoSis = { "currentPage": 1, "totalPages": 1, "pageSize": 8 };
            break;
        default:
            paginadoHistoria = { "currentPage": 1, "totalPages": 1, "pageSize": 8 }; 
            break;
    }

    fetch(this.despliegue + '/Menu/CargarVista?vista=' + vista)
        .then(response => response.text())
        .then(html => {
            document.getElementById('contenido').innerHTML = html;
            /*document.getElementById("fecha").value = `${año}-${mes}-${dia}`;*/
            buscarAtenciones();
        })
        .catch(error => console.error('Error al cargar la vista:', error));

    this.moduloAuditoria = vista;
}

function toggleMenu() {
    const sidebarMenu = document.getElementById('sidebarMenu');
    sidebarMenu.classList.toggle('collapsed');
    const menuTextElements = document.querySelectorAll('.menu-text, .submenu-text');
    menuTextElements.forEach(text => text.style.display = sidebarMenu.classList.contains('collapsed') ? 'none' : 'inline');
}

function mostrarOcultarSubMenu(subMenuId) {
    const subMenu = document.getElementById(subMenuId);
    subMenu.classList.toggle('show');
}

async function buscarAtenciones(event) {
    try {
        if (event) 
            event.preventDefault();        

        document.getElementById("loadingOverlay").classList.remove("d-none");
        var habilitarFecha = false;
       
        //if (!document.getElementById("fecha").value) {
        //    document.getElementById("alertMessage").classList.remove("d-none");
        //    return;
        //} else {
        //    document.getElementById("alertMessage").classList.add("d-none");
        //}

        var _currentPage = 1;
        var _pageSize = 1;
        switch (this.moduloAuditoria) {
            case 'HistoriaClinicas':
                _currentPage = paginadoHistoria.currentPage;
                _pageSize = paginadoHistoria.pageSize;
                break;
            case 'AuditoriaOrdenes':
                _currentPage = paginadoOrden.currentPage;
                _pageSize = paginadoOrden.pageSize;
                break;           
            default:
                _currentPage = paginadoHistoria.currentPage;
                _pageSize = paginadoHistoria.pageSize;
                break;
        }
        
        const parametros = {
            historiaClinica: document.getElementById("historiaClinica").value,
            documentoIdentidad: document.getElementById("documentoIdentidad").value,
            nombre: document.getElementById("nombre").value,
            codigoEstado: document.getElementById("estado").value,
            page: _currentPage,
            pageSize: _pageSize
        };

        const queryString = new URLSearchParams(parametros).toString();
        const response = await fetch(`${this.despliegue}/Menu/${this.moduloAuditoria}?${queryString}`);
        const data = await response.json();

        if (response.ok) {
            updateTable(data.pacientes);
            updatePagination(data.paginacion);
        } else {
            console.error("Error en la solicitud:", data);
        }

    } catch (error) {
        console.error("Error en la solicitud:", error);
    } finally {
        document.getElementById("loadingOverlay").classList.add("d-none");
    }
}

async function sincronizarAtenciones() {
    try {
        document.getElementById("loadingOverlay").classList.remove("d-none");
        var metodoSincronizar = this.moduloAuditoria == "AuditoriaTriaje" ? "CargarAtencionesSinCuentaTriaje" : "CargarEvaluacionesOrdenes";
        const response = await fetch(this.desplieguePost + '/' + metodoSincronizar, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({})
        });
        document.getElementById("loadingOverlay").classList.add("d-none");
        mostrarModalSincronizar("modalProcesando", "Sincronización completada exitosamente.", 1000);        
    } catch (error) {
        mostrarModal("modalAdvertencia", "Ocurrió un error al intentar sincronizar las atenciones.", 1000);
        return false;
    }
}

async function sincronizarAtencionesAdmin() {
    try {
        document.getElementById("loadingOverlay").classList.remove("d-none");
        var metodoSincronizar = "CargarAtencionesSinCuentaTriaje";
        const response = await fetch(this.desplieguePost + '/' + metodoSincronizar, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({})
        });
        document.getElementById("loadingOverlay").classList.add("d-none");
        Swal.fire({
            title: '¡Éxito!',
            text: 'Sincronización completada exitosamente.',
            icon: 'success',
            confirmButtonText: 'OK'
        });
    } catch (error) {
        mostrarModal("modalAdvertencia", "Ocurrió un error al intentar sincronizar las atenciones.", 1000);
        return false;
    }
}

async function asignacionAutomaticaAtenciones() {
    try {
        document.getElementById("loadingOverlay").classList.remove("d-none");
        var metodoSincronizar = "AsignacionAutomatica";
        const response = await fetch(this.desplieguePost + '/' + metodoSincronizar, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({})
        });
        document.getElementById("loadingOverlay").classList.add("d-none");
        Swal.fire({
            title: '¡Éxito!',
            text: 'Asignación completada exitosamente.',
            icon: 'success',
            confirmButtonText: 'OK'
        });
        /*mostrarModalSincronizar("modalProcesando", "Asignación completada exitosamente.", 1000);*/
    } catch (error) {
        mostrarModal("modalAdvertencia", "Ocurrió un error al intentar asignar las atenciones.", 1000);
        return false;
    }
}

function mostrarModalSincronizar(idModal, mensaje, tiempo = 4000) {
    const modal = document.getElementById(idModal);
    if (modal) {
        const modalBody = modal.querySelector(".modal-body p");
        modalBody.innerHTML = mensaje;
        const bootstrapModal = new bootstrap.Modal(modal);
        bootstrapModal.show();
        setTimeout(() => {
            bootstrapModal.hide();
            this.buscarAtenciones();
        }, tiempo);
    } else {
        console.error(`Modal con ID "${idModal}" no encontrado.`);
    }
}


function mostrarModal(idModal, mensaje, tiempo = 3000) {
    var titulo = '¡Advertencia!';
    var icono = 'warning';
    if (idModal == 'modalAdvertencia') {
        titulo = '¡Advertencia!';
        icono = 'warning';
    }
    if (idModal == 'modalError') {
        titulo = '¡Error!';
        icono = 'error';
    }
    if (idModal == 'modalGuardadoExitoso') {
        titulo = '¡Exito!';
        icono = 'success';
    }
    if (idModal == 'modalInformacion') {
        titulo = '¡Información!';
        icono = 'info';
    }

    Swal.fire({
        title: titulo,
        text: mensaje,
        icon: icono,
        confirmButtonText: 'OK'
    });
}

async function limpiarAtenciones() {
    try {
        document.getElementById("loadingOverlay").classList.remove("d-none");
        document.getElementById("fecha").value = `${año}-${mes}-${dia}`;        
        const fecha = document.getElementById("fecha").value;
  
        if (!fecha) {
            document.getElementById("alertMessage").classList.remove("d-none");
            return;
        } else {
            document.getElementById("alertMessage").classList.add("d-none");
        }

        document.getElementById("nombre").value = "";
        document.getElementById("historiaClinica").value = "";
        document.getElementById("documentoIdentidad").value = "";
        document.getElementById("nombre").value = "";
        document.getElementById("nroCuenta").value = "";
        document.getElementById("estado").value = "";
        document.getElementById("tipoServicio").value = 0

        const parametros = {
            ModuloArchivoClinico: moduloHistoriaClinicas,
            tipoServicio: 0,
            fecha: fecha,
            historiaClinica: '',
            nroCuenta: '',
            documentoIdentidad: '',
            nombre: '',
            codigoEstado: 0,
            page: 1,
            pageSize: 8
        };
        const queryString = new URLSearchParams(parametros).toString();
        const response = await fetch(`${this.despliegue}/Menu/${this.moduloAuditoria}?${queryString}`);
        const data = await response.json();

        if (response.ok) {
            updateTable(data.pacientes);
            updatePagination(data.paginacion);
        } else {
            console.error("Error al limpiar:", data);
        }

    } catch (error) {
        console.error("Error al limpiar:", error);
    } finally {
        document.getElementById("loadingOverlay").classList.add("d-none");
    }
}
function updateTable(data) {
    const tableBody = document.querySelector("table tbody");
    tableBody.innerHTML = '';

    const mensajeNoResultados = document.getElementById("MensajeNoResultados");
    mensajeNoResultados.style.display = data.length === 0 ? 'block' : 'none';

    if (data.length === 0) return;

    const fragment = document.createDocumentFragment();

    data.forEach(historia => {
        const row = document.createElement('tr');

        var iconColor = "#0a58ca", borderColor = "#0a58ca", icon = "3p";

        if (moduloHistoriaClinicas !== 'AuditoriaTriaje') {
            if (historia.auditoriaCodigoEstado === 3) {
                iconColor = "red";
                borderColor = "red";
                icon = "warning";
            } else if (historia.auditoriaCodigoEstado === 0 || historia.auditoriaCodigoEstado === 1) {
                iconColor = "#606060";
                borderColor = "#606060";
            }
        }

        const styleCampoCuenta = historia.cuenta_atencion_id ? "campo-verde" : "campo-grilla";
        let btnsAccion = '';

        if (moduloHistoriaClinicas == 'AuditoriaOrdenes' || moduloHistoriaClinicas == 'AuditoriaCuentas') {
            let textFuente = '', btnFuente = 'btn-secondary', textEstado = historia.auditoria_estado;
            let tituloBtn = 'Actualizar Auditoría';

            if (historia.auditoria_codigo_estado === 2) {
                btnFuente = 'btn-primary'
                textFuente = 'text-primary';
            } else if (historia.auditoria_codigo_estado === 3) {
                textFuente = 'text-danger';
                btnFuente = 'btn-danger'
                tituloBtn = 'Subsanar Observación';
                if (historia.auditoria_triaje_subsana_obs) {
                    textFuente = 'text-success';
                    textEstado = 'Subsanado';
                    btnFuente = 'btn-success';
                    tituloBtn = 'Actualizar Subsanación';
                }
            }

            if (historia.auditoria_codigo_estado !== 3) {
                btnsAccion = `
                <td class="text-center">
                    <div class="d-flex justify-content-between w-100 gap-2">
                        <button type="button" class="btn btn-primary w-100 me-2 d-flex align-items-center justify-content-center gap-2 responsive-btn"
                            onclick="abrirModalDetalle('${historia.atencion_id_eess}', '${moduloHistoriaClinicas}')"
                            data-bs-toggle="tooltip" data-bs-placement="top" title="Actualizar Auditoría">
                            <i class="material-icons">edit</i>
                        </button>
                    </div>
                </td>`;
            } else {
                btnsAccion = `
                <td class="text-center">
                    <div class="d-flex justify-content-between w-100 gap-2">
                        <button type="button" class="btn btn-danger w-100 me-2 d-flex align-items-center justify-content-center gap-2 responsive-btn"
                            onclick="abrirModalDetalle('${historia.atencion_id_eess}', '${moduloHistoriaClinicas}')"
                            data-bs-toggle="tooltip" data-bs-placement="top" title="Subsanar Observación">
                            <i class="material-icons">edit_note</i>
                        </button>
                    </div>
                </td>`;
            }       
                
        } else {
            btnsAccion = ``;
        }


        let extraColumns = '';
        switch (moduloHistoriaClinicas) {
            case "AuditoriaCuentas":
                extraColumns = `
                    <td class="small campo-grilla">${historia.fuente_financiamiento ?? ""}</td>
                    <td class="small campo-grilla">${historia.auditoria_estado}</td>
                    <td class="small campo-grilla">${historia.auditoria_usuario ?? ""}</td>`;       
                break;

            case "HistoriaClinicas":
                let textFuente = '', btnFuente = 'btn-secondary', textEstado = historia.estado_historia;
                let tituloBtn = 'Actualizar Historia';

                if (historia.codigo_estado_historia === 2) {
                    btnFuente = 'btn-primary'
                    textFuente = 'text-primary';
                } else if (historia.codigo_estado_historia === 3) {
                    textFuente = 'text-danger';
                    btnFuente = 'btn-danger'
                    tituloBtn = 'Subsanar Observación';
                }

                extraColumns = `
                    <td class="small campo-grilla">${historia.direccion ?? ""}</td>
                    <td class="small campo-grilla">${historia.correo ?? ""}</td>
                    <td class="small campo-grilla ${textFuente}">${textEstado}</td>`;                

                btnsAccion = `
                <td class="text-center">
                    <div class="d-flex justify-content-between w-100 gap-2">
                        <button type="button" class="btn ${btnFuente} w-100 me-2 d-flex align-items-center justify-content-center gap-2 responsive-btn"
                            onclick="abrirModalDetalle('${historia.atencion_id_eess}', '${moduloHistoriaClinicas}')"
                            data-bs-toggle="tooltip" data-bs-placement="top" title="${tituloBtn}">
                            <i class="material-icons">edit</i>
                        </button>
                    </div>
                </td>`;

                break;

            default: // Orden

                extraColumns = `
                    <td class="small campo-grilla">${historia.direccion ?? ""}</td>
                    <td class="small campo-grilla">${historia.correo}</td>`;       
                break;
        }

        row.innerHTML = `
            <td class="small campo-grilla">${historia.historia_clinica_id}</td>
            <td class="small campo-grilla">${historia.numero_historia}</td>
            <td class="small campo-grilla">${historia.tipos_documento} ${historia.numero_documento}</td>
            <td class="small campo-grilla">
                ${historia.apellido_paterno} ${historia.apellido_materno} ${historia.nombres}
            </td>
            <td class="small campo-grilla">${historia.fecha_nacimiento}</td>                    
            <td class="small campo-grilla">${historia.tipo_sexo}</td>
            ${extraColumns}
            ${btnsAccion}`;

        fragment.appendChild(row);
    });

    tableBody.appendChild(fragment);
}


function updatePagination(paginacion) {
    const paginationElement = document.getElementById("paginationContainer");
    if (!paginacion.currentPage || !paginacion.totalPages) {
        paginationElement.style.display = "none"; //totalRecords
        return;
    }
    currentPage = paginacion.currentPage;
    totalPages = paginacion.totalPages;
    totalRecords = paginacion.totalRecords;
    paginationElement.style.display = "block";
    document.getElementById("infoPaginacion").textContent = `Página ${currentPage} de ${totalPages}`;
    document.getElementById("totalRegistros").textContent = `Total de registros: ${totalRecords}`;
    document.getElementById("paginaAnterior").classList.toggle("disabled", currentPage === 1);
    document.getElementById("PaginaSiguiente").classList.toggle("disabled", currentPage === totalPages);
}

async function paginaAnterior() {
    console.log(this.moduloHistoriaClinicas);    
    var _currentPage = 1;
    switch (this.moduloHistoriaClinicas) {
        case 'HistoriaClinicas':
            _currentPage = paginadoHistoria.currentPage;
            if (_currentPage > 1) {
                _currentPage--;
                paginadoHistoria.currentPage = _currentPage;
            }
            break;
        case 'AuditoriaOrdenes':
            _currentPage = paginadoOrden.currentPage;
            if (_currentPage > 1) {
                _currentPage--;
                paginadoOrden.currentPage = _currentPage;
            }
            break;
        case 'AuditoriaCuentas':
            _currentPage = paginadoCuenta.currentPage;
            if (_currentPage > 1) {
                _currentPage--;
                paginadoCuenta.currentPage = _currentPage;
            }
            break;
        case 'AuditoriaFUA':
            _currentPage = paginadoSis.currentPage;
            if (_currentPage > 1) {
                _currentPage--;
                paginadoSis.currentPage = _currentPage;
            }
            break;
        default:
            _currentPage = paginadoHistoria.currentPage;
            if (_currentPage > 1) {
                _currentPage--;
                paginadoHistoria.currentPage = _currentPage;
            }
            break;
    }
    console.log(_currentPage);
    await buscarAtenciones();
}

async function paginaSiguiente() {
    switch (this.moduloHistoriaClinicas) {
        case 'HistoriaClinicas':
            if (paginadoHistoria.currentPage < totalPages) {
                paginadoHistoria.currentPage++;
                await buscarAtenciones();
            }
            break;
        case 'AuditoriaOrdenes':
            if (paginadoOrden.currentPage < totalPages) {
                paginadoOrden.currentPage++;
                await buscarAtenciones();
            }
            break;
        case 'AuditoriaCuentas':
            if (paginadoCuenta.currentPage < totalPages) {
                paginadoCuenta.currentPage++;
                await buscarAtenciones();
            }
            break;
        case 'AuditoriaFUA':
            if (paginadoSis.currentPage < totalPages) {
                paginadoSis.currentPage++;
                await buscarAtenciones();
            }
            break;
        default:
            if (paginadoHistoria.currentPage < totalPages) {
                paginadoHistoria.currentPage++;
                await buscarAtenciones();
            }
            break;
    }
}
function emitirFua(id) {
    document.getElementById("idAtencionSeleccionadaHidden").value = id;
    Swal.fire({
        title: 'Confirmación',
        text: '¿Desea emitir el Fua de la atención?',
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            btnConfirmarFUA();
        };
    });
}

function consultarFua(id, guid) {
    console.log(id);
    Swal.fire({
        title: 'Confirmación',
        text: '¿Desea consultar el estado del fua enviado?',
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            btnConsultarFUA(id, guid);
        };
    });
}

async function btnConsultarFUA(id, guid) {
    const url = `${this.despliegue}/Atencion/ConsultarFua?id=${id}&guidFua=${encodeURIComponent(guid)}`;
    try {
        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        });

        console.log(response);
        if (!response.ok) {
            const errorData = await response.json();
            mostrarModal("modalAdvertencia", errorData.message, 2000);
            return;
        }

        const data = await response.json();
        mostrarModal("modalGuardadoExitoso", "FUA consultado con éxito", 1000);
        $('#modalConfirmacion').modal('hide');
        await buscarAtenciones();
    } catch (error) {
        alert("Ocurrió un error al intentar emitir la FUA.");
    }
}

function abrirDetalleConSwal(idAtencion) {
    var url = `${window.appConfig.apiUrl}/Fua/reporte?idAtencion=${idAtencion}`;

    Swal.fire({
        title: 'Previsualizar FUA',
        html: `<iframe src="${url}" width="100%" height="700px" frameborder="0"></iframe>`,
        showCloseButton: true,
        showConfirmButton: false,
        width: '80%',
        padding: '2em',
        customClass: {
            popup: 'bg-white', 
            content: 'p-0', 
        },
        didOpen: () => {
        },
    });
}

async function btnConfirmarFUA() {
    var id = document.getElementById("idAtencionSeleccionadaHidden").value;   

    const url = `${this.despliegue}/Atencion/EmitirFua?id=${id}`; 
    try {
        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        });

        console.log(response);
        if (!response.ok) {
            const errorData = await response.json();
            mostrarModal("modalAdvertencia", errorData.message, 2000);
            return;
        }

        const data = await response.json();
        mostrarModal("modalGuardadoExitoso", "FUA emitida con éxito", 1000);
        $('#modalConfirmacion').modal('hide');
        await buscarAtenciones();
    } catch (error) {
        alert("Ocurrió un error al intentar emitir la FUA.");
    }
}

function abrirModalDetalle(idAtencionEESS) {
    if (moduloHistoriaClinicas == "AuditoriaTriaje") {
        var url = this.despliegue + '/Atencion/Triaje?id=' + idAtencionEESS;

        document.getElementById("loadingEvaluacion").classList.remove("d-none");
        $('#iframeDetalleTriaje').on('load', function () {
            document.getElementById("loadingEvaluacion").classList.add("d-none");
        });

        $('#iframeDetalleTriaje').attr('src', url);
        $('#modalDetalleTriaje').modal('show');        

        $('#modalDetalleTriaje').on('hidden.bs.modal', async function () {
            await buscarAtenciones();
        });        
    }
    else if (moduloHistoriaClinicas == "AuditoriaOrdenes") {
        var url = this.despliegue + '/Atencion/Orden?id=' + idAtencionEESS;

        document.getElementById("loadingEvaluacion").classList.remove("d-none");
        $('#iframeDetalleOrden').on('load', function () {
            document.getElementById("loadingEvaluacion").classList.add("d-none");
        });

        $('#iframeDetalleOrden').attr('src', url);
        $('#modalDetalleOrden').modal('show');
        $('#modalDetalleOrden').on('hidden.bs.modal', async function () {
            await buscarAtenciones();
        });
    }
    else if (moduloHistoriaClinicas == "AuditoriaCuentas") {
        var url = this.despliegue + '/Atencion/Cuenta?id=' + idAtencionEESS;

        document.getElementById("loadingEvaluacion").classList.remove("d-none");
        $('#iframeDetalleCuenta').on('load', function () {
            document.getElementById("loadingEvaluacion").classList.add("d-none");
        });

        $('#iframeDetalleCuenta').attr('src', url);
        $('#modalDetalleCuenta').modal('show');

        $('#modalDetalleCuenta').on('hidden.bs.modal', async function () {
            await buscarAtenciones();
        });
    }
    else {
        var url = this.despliegue + '/Atencion/FUA?id=' + idAtencionEESS;
        $('#iframeDetalleCuenta').attr('src', url);
        $('#modalDetalleCuenta').modal('show');
    }

    document.getElementById("idAtencionSeleccionadaHidden").value = idAtencionEESS
}

function btnActualizarTriaje() {
    Swal.fire({
        title: 'Confirmación',
        text: '¿Desea grabar la auditoría al triaje?',
        icon: 'info',
        showCancelButton: true, 
        confirmButtonText: 'OK', 
        cancelButtonText: 'Cancelar' 
    }).then((result) => {
        if (result.isConfirmed) {
            btnConfirmarTriaje();
        };
    });

}

function btnActualizarCuenta() {
    Swal.fire({
        title: 'Confirmación',
        text: '¿Desea grabar la auditoría a la cuenta?',
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            btnConfirmarCuenta();
        };
    });

    /*$('#modalConfirmacion').modal('show');*/
}

async function btnActualizarOrden() {
    /*$('#modalConfirmacion').modal('show');*/
    const confirmResult = await Swal.fire({
        title: 'Recargar',
        text: `¿Desea recargar las evaluaciones ?`,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: 'Sí, guardar',
        cancelButtonText: 'No, cancelar',
    });

    if (!confirmResult.isConfirmed) {
        return false;
    }

    await this.btnConfirmarOrden();
}

async function btnConfirmarTriaje() {
    console.log('dasdasds');
    $('#modalConfirmacion').modal('hide');
    var iframe = document.getElementById('iframeDetalleTriaje');

    if (iframe && iframe.contentWindow) {
        if (typeof iframe.contentWindow.Actualizar === "function") {
            try {
                const resultado = await iframe.contentWindow.Actualizar();
                if (resultado === true) {
                    await new Promise(resolve => setTimeout(resolve, 3000));
                    $('#modalDetalleTriaje').modal('hide');
                } else {
                    console.warn("La función Actualizar no devolvió true.");
                }
            } catch (error) {
                console.error("Error al ejecutar Actualizar:", error);
            }
        } else {
            console.error("La función Actualizar no está definida en el iframe.");
        }
    } else {
        console.error("El iframe no está disponible o aún no ha cargado.");
    }
}

async function btnConfirmarOrden() {
    $('#modalConfirmacion').modal('hide');
    const atencionId = document.getElementById("idAtencionSeleccionadaHidden").value;
    var url = this.despliegue + '/Atencion/Orden?id=' + atencionId;
    $('#iframeDetalleOrden').attr('src', url);
    $('#modalDetalleOrden').modal('show'); 
   
}

async function btnConfirmarCuenta() {
    $('#modalConfirmacion').modal('hide');
    var iframe = document.getElementById('iframeDetalleCuenta');

    if (iframe && iframe.contentWindow) {
        if (typeof iframe.contentWindow.Actualizar === "function") {
            try {
                const resultado = await iframe.contentWindow.Actualizar();
                if (resultado === true) {
                    await new Promise(resolve => setTimeout(resolve, 3000));
                    $('#modalDetalleCuenta').modal('hide');
                } else {
                    console.warn("La función Actualizar no devolvió true.");
                }
            } catch (error) {
                console.error("Error al ejecutar Actualizar:", error);
            }
        } else {
            console.error("La función Actualizar no está definida en el iframe.");
        }
    } else {
        console.error("El iframe no está disponible o aún no ha cargado.");
    }
}


async function btnBuscarAtencionesGrid() {
    var _currentPage = 1;
    switch (this.moduloAuditoria) {
        case 'AuditoriaTriaje':
            paginadoHistoria.currentPage = _currentPage;
            break;
        case 'AuditoriaOrdenes':
            paginadoOrden.currentPage = _currentPage;
            break;
        case 'AuditoriaCuentas':
            paginadoCuenta.currentPage = _currentPage;
            break;
        case 'AuditoriaFUA':
            paginadoSis.currentPage = _currentPage;
            break;
        default:
            paginadoHistoria.currentPage = _currentPage;
            break;
    }
    this.buscarAtenciones();
}

