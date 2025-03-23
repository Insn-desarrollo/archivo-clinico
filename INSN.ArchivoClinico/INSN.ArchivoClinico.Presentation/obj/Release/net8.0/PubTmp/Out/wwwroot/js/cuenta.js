var despliegue = "/apwAuditoria";
var desplieguePost = "/apwAuditoria";

$(document).ready(function () {    
    const codigoEstado = parseInt(document.getElementById("AuditoriaCuentaCodigoEstadoHidden").value);
    setEstadoAuditoria(codigoEstado);
});

function setEstadoAuditoria(codigoEstado) {
    const correctoRadio = document.getElementById("AuditoriaCuentaCorrecto");
    const observadoRadio = document.getElementById("AuditoriaCuentaObservado");
    const observacionContainer = document.getElementById("AuditoriaObservacionContainer");

    if (codigoEstado === 2) {
        correctoRadio.checked = true;
        observacionContainer.style.display = "none";
    } else if (codigoEstado === 3) {
        observadoRadio.checked = true;
        observacionContainer.style.display = "block"; 
    } else {
        correctoRadio.checked = false;
        observadoRadio.checked = false;
        observacionContainer.style.display = "none";
    }
}


function recargarTiposFinanciamiento() {
    const fuenteFinanciamientoId = document.getElementById("FuenteFinanciamientoId").value;
    const tipoFinanciamientoSelect = document.getElementById("TipoFinanciamientoId");

    fetch(`${this.despliegue}/Atencion/ObtenerTiposFinanciamiento?fuenteFinanciamientoId=${fuenteFinanciamientoId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Error al cargar los tipos de financiamiento");
            }
            return response.json();
        })
        .then(data => {
            tipoFinanciamientoSelect.innerHTML = "";
            data.forEach(tipo => {
                const option = document.createElement("option");
                option.value = tipo.tipoFinanciamientoId; 
                option.text = tipo.descripcion; 
                tipoFinanciamientoSelect.add(option);
            });
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

//function mostrarModal(idModal, mensaje, tiempo = 3000) {
//    const modal = document.getElementById(idModal);
//    if (modal) {
//        const modalBody = modal.querySelector(".modal-body p");
//        modalBody.innerHTML = mensaje;
//        const bootstrapModal = new bootstrap.Modal(modal);
//        bootstrapModal.show();
//        setTimeout(() => {
//            bootstrapModal.hide();
//        }, tiempo);
//    } else {
//        console.error(`Modal con ID "${idModal}" no encontrado.`);
//    }
//}

function mostrarModal(idModal, mensaje, tiempo = 4000) {
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


function validarLongitud(input, maxLength) {
    if (input.value.length > maxLength) {
        input.value = input.value.slice(0, maxLength);
    }
}

function btnActualizar() {
    const idAtencionHidden = document.getElementById('idAtencionHidden').value;
    // Recopilar los datos del formulario
    const data = {
        atencionId: idAtencionHidden,
        auditoriaUsuario: 'ADMIN'
    };

    fetch(this.desplieguePost + 'AtencionCEAuditoria', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(data)
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            }
            throw new Error('Error en la solicitud');
        })
        .then(result => {
            mostrarMensaje('La auditoria de la atención se ha completado correctamente.', 'success');
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Hubo un problema al realizar la actualización.');
        });
}

function btnSalir() {
    const modal = window.parent.bootstrap.Modal.getInstance(window.parent.document.getElementById('modalDetalleTriaje'));
    modal.hide(); 
}

function btnFormatos() {
    $("#formatosModal").modal('show');
}
function btnConsultas() {
    $("#consultasModal").modal('show');
}

function AbrirObservacionEvaluacion() {
    const correcto = document.getElementById("EvaluacionCorrecto").checked;
    const observacionContainer = document.getElementById("AuditoriaEvaluacionObservacionContainer");

    if (correcto) {
        observacionContainer.style.display = "none";
    } else {
        observacionContainer.style.display = "block";
    }
}

function formatToLocalDate(fecha) {
    if (!fecha) return ""; 
    const date = new Date(fecha);
    if (isNaN(date.getTime())) {
        console.error("Fecha no válida:", fecha);
        return ""; 
    }
    const año = date.getFullYear();
    const mes = String(date.getMonth() + 1).padStart(2, '0'); 
    const dia = String(date.getDate()).padStart(2, '0'); 

    return `${año}-${mes}-${dia}`; 
}

function aprobarItemAuditoria(idItemAuditoria, puntoCarga) {
    document.getElementById('auditoriaItemHidden').value = idItemAuditoria;
    document.getElementById('auditoriaPuntoCargaHidden').value = puntoCarga;
    Swal.fire({
        title: 'Aprobar',
        text: '¿Estás seguro de grabar la aprobación?',
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: 'OK',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            ActualizarConfirmacion();
        };
    });

/*    $('#confirmacionModal').modal('show');*/
}

function cargarItemAuditoriaModal(idItemAuditoria, puntoCarga) {
    document.getElementById('itemAuditoriaCantidad').disabled = true;
    const modal = document.getElementById('itemAuditoriaModal');
    modal.setAttribute('data-id', idItemAuditoria);

    fetch(`${this.despliegue}/Atencion/DetalleItemAuditoria?id=${idItemAuditoria}&puntoCarga=${encodeURIComponent(puntoCarga)}`, {
        method: 'GET'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok: ' + response.statusText);
            }
            return response.json(); // Convertir la respuesta a JSON
        })
        .then(data => {
            if (data.success) {
                if (data.data.auditoriaPuntoCarga === "FARMACIA") {
                    document.getElementById('itemAuditoriaGrupo').textContent = `${data.data.auditoriaPuntoCarga}`;
                } else {
                    document.getElementById('itemAuditoriaGrupo').textContent = `${data.data.auditoriaPuntoCarga} - ${data.data.grupo}`;
                }
                document.getElementById("itemAuditoriaCantAsignada").checked = false;
                document.getElementById('itemAuditoriaCodDescripcion').value = `${data.data.codigoItem} - ${data.data.item}`;
                document.getElementById('itemAuditoriaCie10').value = `${data.data.cie10} - ${data.data.diagnostico}`;
                document.getElementById('itemAuditoriaCantidad').value = data.data.auditoriaCantidad;
                document.getElementById('itemAuditoriaDetalleObservacion').value = data.data.auditoriaObservacion || '';

                // Almacenar auditoriaPuntoCarga en el input hidden
                document.getElementById('auditoriaPuntoCargaHidden').value = puntoCarga;
            }
            else {
                console.error('Error al obtener el item', data.message);
            }
        })
        .catch(error => console.error('Error al cargar los datos del item:', error));
}


async function obtenerEvaluacionDetalles(evaluacionId) {
    try {
        document.getElementById("loadingOverlay").classList.remove("d-none");
        const response = await fetch(`${this.despliegue}/Atencion/DetalleEvaluacion/${evaluacionId}`);
        if (response.ok) {
            const evaluacionJson = await response.json();            
            const datosEvaluacion = evaluacionJson.data;
            document.getElementById("FechaEvaluacion").value = formatToLocalDate(datosEvaluacion.fechaEvaluacion);
            document.getElementById("HoraEvaluacion").value = datosEvaluacion.horaEvaluacion ?? "";
            document.getElementById("ServicioEESS").value = datosEvaluacion.servicioEESS ?? "";
            document.getElementById("AuditoriaEvaluacionCodigoEstadoHidden").value = datosEvaluacion.auditoriaCodigoEstado ?? "";
            cambiarEstadoEvaluacion(evaluacionId, datosEvaluacion.auditoriaCodigoEstado);
            const correctoRadio = document.getElementById("EvaluacionCorrecto");
            const observadoRadio = document.getElementById("EvaluacionObservado");
            const observacionContainer = document.getElementById("AuditoriaEvaluacionObservacionContainer");
            if (datosEvaluacion.auditoriaCodigoEstado == 2)
            {
                correctoRadio.checked = true;
                observacionContainer.style.display = "none";
            } else if (datosEvaluacion.auditoriaCodigoEstado === 3) {
                observadoRadio.checked = true;
                observacionContainer.style.display = "block"; 
            } else {
                correctoRadio.checked = false;
                observadoRadio.checked = false;
                observacionContainer.style.display = "none";
            }            

            document.getElementById("AuditoriaEvaluacionObservacion").value = datosEvaluacion.auditoriaObservacion ?? "";
            document.getElementById("idEvaluacionHidden").value = datosEvaluacion.evaluacionId ?? "";

            const diagnosticos = datosEvaluacion.diagnosticos;
            const diagnosticosTableBody = document.getElementById('diagnosticosTableBody');
            diagnosticosTableBody.innerHTML = '';
            diagnosticos.forEach(d => {
                const row = `
                <tr>
                    <td>${d.numeroOrden}</td>
                    <td>${d.codigoDiagnostico}</td>
                    <td>${d.tipoDiagnostico}</td>
                    <td>${d.diagnostico}</td>
                    <td>${d.esPrincipal ? 'Sí' : 'No'}</td>
                </tr>`;
                diagnosticosTableBody.innerHTML += row;
            });

            // Actualizar Procedimientos
            const procedimientos = datosEvaluacion.examenesAuxiliares;
            const procedimientosTableBody = document.getElementById('procedimientosTableBody');
            procedimientosTableBody.innerHTML = '';

            procedimientos.forEach(p => {
                let estadoClase = '';
                if (p.auditoriaEstado === 'Aceptado') {
                    estadoClase = 'text-primary';
                } else if (p.auditoriaEstado === 'Observado') {
                    estadoClase = 'text-danger';
                } else {
                    estadoClase = '';
                }

                const row = `
        <tr>
            <td>
                <div class="d-flex gap-2">
                    <button class="insn-futuristic-btn insn-futuristic-btn-primary" title="Confirmar" onclick="aprobarItemAuditoria(${p.evaluacionProcedimientoId}, 'PROCEDIMIENTO')">
                        <span class="material-icons">check_circle</span>
                    </button>
                    <button class="insn-futuristic-btn insn-futuristic-btn-danger" title="Advertencia" data-bs-toggle="modal" data-bs-target="#itemAuditoriaModal" onclick="cargarItemAuditoriaModal(${p.evaluacionProcedimientoId}, 'PROCEDIMIENTO')">
                        <span class="material-icons">warning</span>
                    </button>
                </div>
            </td>
            <td>${p.servicio || 'N/A'}</td>
            <td>${p.grupo || 'N/A'}</td>
            <td>${p.codigoProcedimiento || 'N/A'}</td>
            <td>${p.procedimiento || 'N/A'}</td>
            <td>${p.codigoDiagnostico || 'N/A'}</td>
            <td>${p.cantidadPrescrita || 0}</td>
            <td class="${estadoClase}">${p.auditoriaEstado || 'Pendiente'}</td>
        </tr>`;
                procedimientosTableBody.innerHTML += row;
            });

            const medicamentos = datosEvaluacion.medicamentos;
            const medicamentosTableBody = document.getElementById('medicamentosTableBody');
            medicamentosTableBody.innerHTML = '';

            medicamentos.forEach(m => {
                let estadoClase = '';
                if (m.auditoriaEstado === 'Aceptado') {
                    estadoClase = 'text-primary'; 
                } else if (m.auditoriaEstado === 'Observado') {
                    estadoClase = 'text-danger'; 
                } else {
                    estadoClase = '';
                }

                const row = `<tr>
                    <td>
                        <div class="d-flex gap-2">
                            <!-- Botón Confirmar -->
                            <button class="insn-futuristic-btn insn-futuristic-btn-primary" 
                                    title="Confirmar" 
                                    onclick="aprobarItemAuditoria(${m.evaluacionMedicamentoId}, 'FARMACIA')">
                                <span class="material-icons">check_circle</span>
                            </button>
                            <!-- Botón Advertencia -->
                            <button class="insn-futuristic-btn insn-futuristic-btn-danger" 
                                    title="Advertencia" 
                                    data-bs-toggle="modal" 
                                    data-bs-target="#itemAuditoriaModal" 
                                    onclick="cargarItemAuditoriaModal(${m.evaluacionMedicamentoId}, 'FARMACIA')">
                                <span class="material-icons">warning</span>
                            </button>
                        </div>
                    </td>
                    <td>${m.codigoMedicamento}</td>
                    <td>${m.medicamento}</td>
                    <td>${m.dosis || ''}</td>
                    <td>${m.dias || ''}</td>
                    <td>${m.codigoDiagnostico || ''}</td>
                    <td>${m.cantidadPrescrita}</td>
                    <td class="${estadoClase}">${m.auditoriaEstado || 'Pendiente'}</td>
                </tr>`;
                medicamentosTableBody.innerHTML += row;
            });



        } else {
            alert("Error al obtener los detalles de la evaluación.");
        }
    } catch (error) {
        console.error("Error al hacer la solicitud:", error);
        alert("Hubo un problema al obtener los detalles.");
    } finally {
        document.getElementById("loadingOverlay").classList.add("d-none");
    }
}

document.getElementById('itemAuditoriaSubmitForm').addEventListener('click', async function (e) {
    e.preventDefault();

    $('#observacionModal').modal('show');

    document.getElementById('actualizarObservacion').addEventListener('click', async function () {
        const modal = document.getElementById('itemAuditoriaModal');
        const idItemAuditoria = modal.getAttribute('data-id');
        const auditoriaPuntoCarga = document.getElementById('auditoriaPuntoCargaHidden').value;

        const cantidad = document.getElementById('itemAuditoriaCantidad');
        const detalleObservacion = document.getElementById('itemAuditoriaDetalleObservacion');

        if (parseFloat(cantidad.value) <= 0 || isNaN(cantidad.value)) {
            mostrarModal("modalAdvertencia", "La cantidad debe ser mayor que 0", 2000);
            return false;
        }

        if (detalleObservacion.value.trim().length === 0) {
            mostrarModal("modalAdvertencia", "Debe registrar el detalle de la observación", 2000);
            return false;
        }

        const data = {
            PuntoCarga: auditoriaPuntoCarga,
            IdItem: idItemAuditoria,
            AuditoriaCantidad: document.getElementById('itemAuditoriaCantidad').value || null, 
            AuditoriaObservacion: document.getElementById('itemAuditoriaDetalleObservacion').value,
            IdEstadoItem: 3,
            AuditoriaUsuario: 'ADMIN'
        };

        try {
            const response = await fetch(desplieguePost + '/ActualizarAuditoriaItem', {
                method: 'POST', 
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                throw new Error(`Error: ${response.status} ${response.statusText}`);
            }

            const result = await response.json();

            $('#observacionModal').modal('hide');
            $('#itemAuditoriaModal').modal('hide');

            const idEvaluacionHidden = document.getElementById('idEvaluacionHidden').value;            
            obtenerEvaluacionDetalles(idEvaluacionHidden);
            mostrarMensaje('La observación se ha completado correctamente.', 'success');
        } catch (error) {
            console.error('Error:', error);
            mostrarMensaje('Hubo un problema al realizar la actualización.', 'danger');
        }
    });
});

function cambiarEstadoEvaluacion(EvaluacionId, AuditoriaEstadoId) {
    if (!EvaluacionId) {
        alert('Por favor, ingrese un ID de evaluación.');
        return;
    }
    if (AuditoriaEstadoId != 2 && AuditoriaEstadoId != 3) {
        return;
    }

    const filas = document.querySelectorAll('#tablaEvaluaciones tbody tr');
    let encontrado = false;

    filas.forEach(fila => {
        const id = fila.getAttribute('data-id');
        if (id != null) {
            if (id === EvaluacionId.toString()) {
                encontrado = true;
                const celdaEstado = fila.querySelector('td:nth-child(4)');
                celdaEstado.textContent = AuditoriaEstadoId == 2 ? 'Aceptado' : 'Observado';
            }
        }
    });
}

//document.getElementById('actualizarConfirmacion').addEventListener('click', async function (e) {
//    e.preventDefault();
//    ActualizarConfirmacion();
//});

async function ActualizarConfirmacion() {
    const idItemAuditoria = document.getElementById('auditoriaItemHidden').value;
    const auditoriaPuntoCarga = document.getElementById('auditoriaPuntoCargaHidden').value;
    const data = {
        PuntoCarga: auditoriaPuntoCarga,
        IdItem: idItemAuditoria,
        AuditoriaCantidad: null,
        AuditoriaObservacion: '',
        IdEstadoItem: 2,
        AuditoriaUsuario: 'ADMIN'
    };

    try {
        const response = await fetch(desplieguePost + '/ActualizarAuditoriaItem', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            throw new Error(`Error: ${response.status} ${response.statusText}`);
        }

        const result = await response.json();

        $('#confirmacionModal').modal('hide');
        const idEvaluacionHidden = document.getElementById('idEvaluacionHidden').value;
        obtenerEvaluacionDetalles(idEvaluacionHidden);
        mostrarMensaje('La aprobación se ha completado correctamente.', 'success');

    } catch (error) {
        console.error('Error:', error);
        alert('Hubo un problema al realizar la actualización.');
    }
}

function mostrarMensaje(mensaje, tipo) {
    const mensajeModalTexto = document.getElementById('mensajeModalTexto');
    const mensajeIcono = document.querySelector('#mensajeModal .modal-body i');
    mensajeModalTexto.textContent = mensaje;
    switch (tipo) {
        case 'success':
            mensajeIcono.className = 'bi bi-check-circle-fill';
            mensajeIcono.style.color = '#28a745'; // Verde para éxito
            break;
        case 'error':
            mensajeIcono.className = 'bi bi-exclamation-circle-fill';
            mensajeIcono.style.color = '#dc3545'; // Rojo para error
            break;
        case 'warning':
            mensajeIcono.className = 'bi bi-exclamation-triangle-fill';
            mensajeIcono.style.color = '#ffc107'; // Amarillo para advertencia
            break;
        default:
            mensajeIcono.className = 'bi bi-info-circle-fill';
            mensajeIcono.style.color = '#007bff'; // Azul para información
    }

    // Mostrar el modal
    $('#mensajeModal').modal('show');
}

document.getElementById('itemAuditoriaCantAsignada').addEventListener('change', function () {
    const itemAuditoriaCantidadInput = document.getElementById('itemAuditoriaCantidad');
    itemAuditoriaCantidadInput.disabled = !this.checked;
});


function AbrirCuentaTriaje() {
    const correcto = document.getElementById("AuditoriaCuentaCorrecto").checked;
    const observacionContainer = document.getElementById("AuditoriaObservacionContainer");
    if (correcto) {
        observacionContainer.style.display = "none";
    } else {
        observacionContainer.style.display = "block";
    }
}

async function Actualizar() {
    try {
        const atencionId = document.getElementById("idAtencionHidden").value;
        const auditoriaCuentaCorrecto = document.getElementById("AuditoriaCuentaCorrecto").checked;
        const auditoriaCuentaObservado = document.getElementById("AuditoriaCuentaObservado").checked;
        const auditoriaObservacion = document.getElementById("AuditoriaCuentaObservacion").value || '';
           
        if (!auditoriaCuentaCorrecto && !auditoriaCuentaObservado) {
            mostrarModal("modalAdvertencia", "Debe seleccionar al menos una opción de auditoría (Cuenta Correcto u Observado).", 2000);
            return false;
        }
        if (auditoriaCuentaObservado && auditoriaObservacion.trim() === '') {
            mostrarModal("modalAdvertencia", "Debe proporcionar una observación.", 2000);
            return false;
        }               

        const data = {
            AtencionId: atencionId,          
            AuditoriaObservacion: auditoriaObservacion,
            AuditoriaCuentaCodigoEstado: auditoriaCuentaCorrecto ? 2 : 3,
            AuditoriaUsuario: 'FCACHAY'
        };

        const response = await fetch(this.desplieguePost + '/ActualizarCuenta', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        if (response.status === 401) {
            try {
                const result = await response.json();
                if (result.redirectUrl) {
                    window.top.location.href = result.redirectUrl; // Redirige en la ventana principal
                } else {
                    window.top.location.href = "/Account/Login"; // Fallback al login por defecto
                }
            } catch (error) {
                console.error("Error al procesar la redirección:", error);
                window.top.location.href = "/Account/Login"; // Fallback al login por defecto
            }
            return;
        }

        if (!response.ok) {
            throw new Error(`Error: ${response.status} ${response.statusText}`);
        }

        const result = await response.json();
        if (result) {           
           mensaje = `Se guardó correctamente la auditoria a la cuenta`;
            mostrarModal("modalGuardadoExitoso", mensaje, 2000);
            return true;
        } else {
            mostrarModal("modalAdvertencia", "Hubo un error al guardar.", 2000);
            return false;
        }
    } catch (error) {
        console.error('Error en Actualizar:', error);
        mostrarModal("modalAdvertencia", "Ocurrió un error al intentar guardar la atención.", 2000);
        return false;
    }
}
