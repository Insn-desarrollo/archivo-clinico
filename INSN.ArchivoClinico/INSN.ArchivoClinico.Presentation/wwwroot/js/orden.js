var despliegue = "/apwArchivoClinico";
var desplieguePost = "/apwArchivoClinico";
var texto_confirmar = 'SI'; //'Sí, guardar';
var texto_cancelar = 'NO'; //' 'No, cancelar'';

$(document).ready(function () {    
    const btnAceptarMasivo = document.getElementById("btnAceptarMasivo");
    const btnEnviarRecetaOrden = document.getElementById("btnEnviarRecetaOrden");

    btnAceptarMasivo.classList.add("d-none");
    btnEnviarRecetaOrden.classList.add("d-none");
});

function setEstadoAuditoria(codigoEstado) {
    const correctoRadio = document.getElementById("AuditoriaTriajeCorrecto");
    const observadoRadio = document.getElementById("AuditoriaTriajeObservado");
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
        confirmButtonText: texto_confirmar,
        cancelButtonText: texto_cancelar,
    }).then((result) => {
        if (result.isConfirmed) {
            this.ActualizarConfirmacionOrden();
        };
    });
}

function cargarItemAuditoriaModal(idItemAuditoria, puntoCarga) {
    fetch(`${this.despliegue}/Atencion/DetalleItemAuditoria?id=${idItemAuditoria}&puntoCarga=${encodeURIComponent(puntoCarga)}`, {
        method: 'GET'
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok: ' + response.statusText);
            }
            return response.json(); 
        })
        .then(data => {
            if (data.success) {
                var auditoriaPuntoCarga = '';
                var itemAuditoriaCodDescripcion = '';
                var itemAuditoriaCie10 = '';
                var cantidadPrescrita = 0;
                var cantidadEntregada = 0;
                var auditoriaCantidad = 0;
                var auditoriaObservacion = '';
                var labelCodDescripcion = 'Medicamento / Insumo';

                console.log(document.getElementById('idEvaluacionHidden').value);
                document.getElementById('auditoriaItemHidden').value = idItemAuditoria;
                document.getElementById('auditoriaPuntoCargaHidden').value = puntoCarga;
                document.getElementById('codigoItemHidden').value = data.data.codigoItem;

                if (data.data.auditoriaPuntoCarga === "FARMACIA") {
                    auditoriaPuntoCarga = `${data.data.auditoriaPuntoCarga}`;
                    labelCodDescripcion = 'Medicamento / Insumo';
                } else {
                    auditoriaPuntoCarga = `${data.data.auditoriaPuntoCarga} - ${data.data.grupo}`;
                    labelCodDescripcion = 'Exámen Auxiliar (CPMS)';
                }

                itemAuditoriaCodDescripcion = `${data.data.codigoItem} - ${data.data.item}`;
                itemAuditoriaCie10 = `${data.data.cie10} - ${data.data.diagnostico}`;
                cantidadPrescrita = data.data.cantidadPrescrita;
                cantidadEntregada = data.data.cantidadEntregada;
                auditoriaCantidad = data.data.auditoriaCantidad;
                auditoriaObservacion = data.data.auditoriaObservacion || '';
                auditoriaOrdenSubsanaObsTexto = data.data.auditoriaOrdenSubsanaObsTexto || '';
                               
                var checkAceptado = data.data.idEstadoAuditoria === 2 ? "checked" : "";
                var checkObservado = data.data.idEstadoAuditoria === 3 ? "checked" : "";
                var displayObservacion = data.data.idEstadoAuditoria === 3 ? "" : " style='display: none;' ";     
                var displaySubsanacion = " style='display: none;"          
                var CheckAuditoriaOrdenSubsanaObs = data.data.auditoriaOrdenSubsanaObs === true ? "checked" : "";
                var displayObsSubsanacion = data.data.auditoriaOrdenSubsanaObs === true ? " style='display: block;' " : " style='display: none;' ";                                
                var readonlyAudiCantidad = (data.data.idEstadoAuditoria === 2 || (data.data.auditoriaPuntoCarga !== "FARMACIA")) ? "readonly" : "";
                
                Swal.fire({
                    title: '<div class="bg-banner text-white d-flex align-items-center justify-content-center py-2 px-3"><h3 class="modal-title" id="itemAuditoriaModalLabel"><i class="material-icons">personal_injury</i>Ítem Auditado</h5></div>',
                    html: `
                        
                        <div class="row m-2">
                            <div class="col-md-12">
                                <div id="MensajesValidacion" class="d-none bg-danger text-white p-3 rounded">
                                    <ul id="ListaValidaciones" class="mb-0"></ul>
                                </div>
                                <br>
                            </div>
                        </div>

                        <div class="row m-2">
                            <div class="col-md-12">
                                  <div class="card border rounded-6 shadow" style="position: relative; overflow: visible;">

                                        <div class="bg-white px-3 py-1" style="position: absolute; top: 0; left: 10px; transform: translateY(-50%); font-size: 1rem; font-weight: bold; border-radius: 5px; color: #4a90e2;">
                                                <i class="material-icons">build</i><label id="itemAuditoriaGrupo">${auditoriaPuntoCarga}</label>                                      
                                        </div>

                                        <div class="card-body">

                                          <div class="row m-2">
                                              <div class="col-md-12">
                                                <div class="card border rounded-6 shadow" style="position: relative; overflow: visible;">
                                                  <div class="px-3 py-1" style="position: absolute; top: 0; left: 10px; transform: translateY(-50%); font-size: 1rem; font-weight: bold; border-radius: 5px; color: #4a90e2;">
                                                  </div>
                                                  <div class="card-body">
                                                    <div class="row mb-3">
                                                      <div class="col-sm-6">
                                                        <label for="itemAuditoriaCodDescripcion" class="col-form-label">${labelCodDescripcion}</label>
                                                        <input type="text" class="form-control" id="itemAuditoriaCodDescripcion" name="itemAuditoriaCodDescripcion" value="${itemAuditoriaCodDescripcion}" readonly>
                                                      </div>
                                                      <div class="col-sm-6">
                                                        <label for="itemAuditoriaCie10" class="col-form-label">CIE 10 - Diagnóstico Relacionado</label>
                                                        <input type="text" class="form-control" id="itemAuditoriaCie10" name="itemAuditoriaCie10" value="${itemAuditoriaCie10}" readonly>
                                                      </div>
                                                    </div>
                                                  </div>
                                                </div>
                                              </div>
                                            </div>


                                            <div class="row m-2">
                                                <div class="col-md-12">
                                                    <div class="card border rounded-6 shadow" style="position: relative; overflow: visible;">
                                                         <div class="px-3 py-1" style="position: absolute; top: 0; left: 10px; transform: translateY(-50%); font-size: 1rem; font-weight: bold; border-radius: 5px; color: #4a90e2;">
                                                         </div>
                                                         <div class="card-body">
                                                            <div class="row m-2 ms-2">
                                                                <div class="col-sm-4 mb-2">
                                                                    <label for="cantidadPrescrita" class="col-form-label">Cantidad Prescrita</label>
                                                                    <input type="text" class="form-control" id="cantidadPrescrita" name="cantidadPrescrita" value="${cantidadPrescrita}" readonly>
                                                                    <small class="text-muted d-block" style="font-size: 0.8rem;">Cantidad Prescrita en la Evaluación</small>
                                                                </div>    
                                                                <div class="col-sm-4 mb-2">
                                                                    <label for="auditoriaCantidad" class="col-form-label">Cantidad Auditada</label>
                                                                    <input type="number" class="form-control" id="auditoriaCantidad" name="auditoriaCantidad" value="${auditoriaCantidad}" min="1" ${readonlyAudiCantidad}>
                                                                    <small class="text-muted d-block" style="font-size: 0.8rem;">Cantidad Registrada por el Auditor</small>
                                                                 </div>                                                               
                                                                 <div class="col-sm-4 mb-2">
                                                                    <label for="cantidadEntregada" class="col-form-label">Cantidad Entregada</label>
                                                                    <input type="text" class="form-control" id="cantidadEntregada" name="cantidadEntregada" value="${cantidadEntregada}" readonly>
                                                                    <small class="text-muted d-block" style="font-size: 0.8rem;">Cantidad Entregada en el servicio de Apoyo al Diagnóstico</small>
                                                                </div>                                                              
                                                            </div>    
                                                            <div class="row m-2 ms-2">
                                                                <div class="col-sm-12 mb-2">

                                                                    <div class="d-flex align-items-center gap-2">

                                                                        <div class="form-check">
                                                                            <input type="radio" class="form-check-input"
                                                                                   id="AuditoriaOrdenCorrecto"
                                                                                   name="AuditoriaOrdenCodigoEstado"
                                                                                   value="2" ${checkAceptado}
                                                                                   onchange="AbrirObservacionOrden()">
                                                                            <label class="form-check-label" for="AuditoriaOrdenCorrecto">Aceptado</label>
                                                                        </div>
                                                                        <div class="form-check">
                                                                            <input type="radio" class="form-check-input"
                                                                                   id="AuditoriaOrdenObservado"
                                                                                   name="AuditoriaOrdenCodigoEstado"
                                                                                   value="3" ${checkObservado}
                                                                                   onchange="AbrirObservacionOrden()">
                                                                            <label class="form-check-label" for="AuditoriaOrdenObservado">Observado</label>
                                                                        </div>

                                                                    </div>



                                                                 </div>  
                                                             </div>   
                                                            <div class="row m-2 ms-2" id="AuditoriaObservacionContainer" ${displayObservacion}>
                                                                <div class="col-sm-12 mb-2">
                                                                    <div class="form-group">
                                                                        <label for="itemAuditoriaDetalleObservacion">Observación:</label>
                                                                        <textarea class="form-control" id="itemAuditoriaDetalleObservacion" maxlength="100" rows="3" aria-describedby="AuditoriaTriajeObservacionHelp">${auditoriaObservacion}</textarea>
                                                                    </div>
                                                                </div>  
                                                             </div>   
                                                             <div class="row m-2 ms-2" id="AuditoriaOrdenSubsanaObsContainer">
                                                                <div class="col-sm-2 mb-2" id="AuditoriaOrdenSubsanaObsCheckContainer" ${displaySubsanacion}>
                                                                    <div class="d-flex justify-content-between w-100 gap-2">
                                                                        <div class="form-check">
                                                                            <input class="form-check-input"
                                                                                   type="checkbox"
                                                                                   id="AuditoriaOrdenSubsanaObs"
                                                                                   name="AuditoriaOrdenSubsanaObs"
                                                                                   style="accent-color: var(--bs-primary);"
                                                                                   onchange="SubsanarObservacion()" ${CheckAuditoriaOrdenSubsanaObs}>
                                                                            <label class="form-check-label text-success fw-bold" for="AuditoriaOrdenSubsanaObs">
                                                                                Subsanar
                                                                            </label>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="col-sm-10 mb-2" id="AuditoriaOrdenSubsanaObsTextoContainer" ${displayObsSubsanacion}>

                                                                    <div class="form-group">
                                                                        <textarea class="form-control text-start align-text-top"
                                                                                  id="AuditoriaOrdenSubsanaObsTexto"
                                                                                  name="AuditoriaOrdenSubsanaObsTexto"
                                                                                  maxlength="100"
                                                                                  rows="2"
                                                                                  aria-describedby="AuditoriaOrdenSubsanaObsTextoHelp">${auditoriaOrdenSubsanaObsTexto}</textarea>
                                                                        <small id="AuditoriaOrdenSubsanaObsTextoHelp" class="form-text text-muted">
                                                                            Ingrese el comentario de la subsanación (máximo 100 caracteres).
                                                                        </small>
                                                                    </div>

                                                                </div>
                                                            </div>

                                                         </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                  </div>

                                <input type="hidden" id="auditoriaPuntoCargaHidden" value="">        
                                                                                                                   
                            </div>
                        </div>

		                <div class="row justify-content-center">
                            <div class="col-md-6">

                                <div class="d-flex gap-2 justify-content-center">
                                    <button type="button" id="btnGuardarOrden" class="btn btn-primary d-flex align-items-center justify-content-center gap-2 responsive-btn" data-bs-toggle="tooltip" data-bs-placement="top" title="Guardar">
                                        <i class="material-icons">save</i> Guardar
                                    </button>
                                    <button type="button" id="btnSalirOrden" class="btn btn-secondary d-flex align-items-center justify-content-center gap-2 responsive-btn" data-bs-dismiss="modal" data-bs-toggle="tooltip" data-bs-placement="top" title="Salir">
                                        <i class="material-icons">logout</i> Salir
                                    </button>                   
                                </div>  

                            </div>
                        </div>

                `,
                    showConfirmButton: false,
                    width: '80%',
                    customClass: {
                        popup: 'swal-wide'
                    },
                    didOpen: () => {                        
                        document.getElementById('btnSalirOrden').addEventListener('click', () => {
                            Swal.close();
                        });
                        document.getElementById('btnGuardarOrden').addEventListener('click', () => {

                            this.btnAceptarOrden();
                        });
                    },
                        preConfirm: () => {
                        const cantidadAuditada = document.getElementById('auditoriaCantidad')?.value;
                        if (!cantidadAuditada) {
                            Swal.showValidationMessage('Por favor, ingrese una cantidad válida.');
                        }
                        return cantidadAuditada; 
                    }
                });

            }
            else {
                console.error('Error al obtener el item', data.message);
            }
        })
        .catch(error => console.error('Error al cargar los datos del item:', error));
}


async function obtenerEvaluacionDetalles(atencionId) {
    console.log(atencionId);
    try {
        document.getElementById("loadingOverlay").classList.remove("d-none");
        const response = await fetch(`${this.despliegue}/Atencion/Evaluaciones/${atencionId}`);
        if (response.ok) {
            const evaluacionJson = await response.json();            
            //const datosEvaluacion = evaluacionJson.data;
            //const btnAceptarMasivo = document.getElementById("btnAceptarMasivo");

            //console.log(datosEvaluacion);

            //const diagnosticos = datosEvaluacion.diagnosticos;
            //const diagnosticosTableBody = document.getElementById('diagnosticosTableBody');
            //diagnosticosTableBody.innerHTML = '';
            //diagnosticos.forEach(d => {
            //    const row = `
            //    <tr>
            //        <td class="text-center">${d.numero_orden}</td>
            //        <td class="text-center">${d.codigo_diagnostico}</td>
            //        <td class="text-center">${d.tipo_diagnostico}</td>
            //        <td class="text-center">${d.diagnostico}</td>
            //        <td class="text-center">${d.es_principal ? 'Sí' : 'No'}</td>
            //    </tr>`;
            //    diagnosticosTableBody.innerHTML += row;
            //});                 



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

async function aceptarMasivo() {
    const evaluacionId = document.getElementById('idEvaluacionEESSHidden').value;
    if (!evaluacionId || evaluacionId.trim() === '') {
        mostrarMensaje('Seleccione una evaluación.', 'warning');
        return false;
    }

    const confirmResult = await Swal.fire({
        title: 'Auditoría masiva',
        text: `¿Estás seguro de auditar y aceptar todos los items?`,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: texto_confirmar,
        cancelButtonText: texto_cancelar,
    });

    if (!confirmResult.isConfirmed) {
        return false;
    }

    const evaluacionEessId = document.getElementById('idEvaluacionEESSHidden').value;
    const data = {
        EvaluacionEESSId: evaluacionEessId,
        AuditoriaUsuario: 'ADMIN'
    };

    try {
        const response = await fetch(desplieguePost + '/AceptacionMasivaItemsEvaluacion', {
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

        Swal.close();

        const evaluacionEessId = document.getElementById('idEvaluacionEESSHidden').value;
        obtenerEvaluacionDetalles(evaluacionEessId);

        const filas = document.querySelectorAll('#tablaEvaluaciones tbody tr');
        let encontrado = false;
        filas.forEach(fila => {
            const id = fila.getAttribute('data-id');
            if (id != null) {
                if (id === evaluacionEessId.toString()) {
                    encontrado = true;
                    const recetaEstado = fila.querySelector('td:nth-child(4)');
                    recetaEstado.textContent = 'Aceptado';

                    const OrdenEstado = fila.querySelector('td:nth-child(5)');
                    OrdenEstado.textContent = 'Aceptado';
                }
            }
        });
        mostrarMensaje('La aceptación masiva se ha completado correctamente.', 'success');
    } catch (error) {
        console.error('Error:', error);
        mostrarMensaje('Hubo un problema al realizar la aceptación masiva.', 'danger');
    }
}


async function registrarReceta() {
    const evaluacionId = document.getElementById('idEvaluacionEESSHidden').value;
    if (!evaluacionId || evaluacionId.trim() === '') {
        mostrarMensaje('Seleccione una evaluación.', 'warning');
        return false;
    }

    const confirmResult = await Swal.fire({
        title: 'Envio de recetas y ordenes auditadas',
        text: `¿Estás seguro de enviar las recetas y ordenes a los servicios de apoyo al diagnóstico?`,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: texto_confirmar,
        cancelButtonText: texto_cancelar,
    });

    if (!confirmResult.isConfirmed) {
        return false;
    }

    const evaluacionEessId = document.getElementById('idEvaluacionEESSHidden').value;
    const data = {
        EvaluacionEESSId: evaluacionEessId
    };

    try {
        const response = await fetch(desplieguePost + '/RegistrarRecetaOrdenApoyoDx', {
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

        Swal.close();

        const evaluacionEessId = document.getElementById('idEvaluacionEESSHidden').value;
        obtenerEvaluacionDetalles(evaluacionEessId);

        const filas = document.querySelectorAll('#tablaEvaluaciones tbody tr');
        let encontrado = false;
        filas.forEach(fila => {
            const id = fila.getAttribute('data-id');
            if (id != null) {
                if (id === evaluacionEessId.toString()) {
                    encontrado = true;
                    const recetaEstado = fila.querySelector('td:nth-child(4)');
                    recetaEstado.textContent = 'Enviado';

                    const OrdenEstado = fila.querySelector('td:nth-child(5)');
                    OrdenEstado.textContent = 'Enviado';
                }
            }
        });

        mostrarMensaje('El envio de las recetas y ordenes se ha completado correctamente.', 'success');
    } catch (error) {
        console.error('Error:', error);
        mostrarMensaje('Hubo un problema al realizar el envio de las recetas y ordenes.', 'danger');
    }


}

async function obtenerResultados() {
    const evaluacionId = document.getElementById('idEvaluacionEESSHidden').value;
    if (!evaluacionId || evaluacionId.trim() === '') {
        mostrarMensaje('Seleccione una evaluación.', 'warning');
        return false;
    }

    const confirmResult = await Swal.fire({
        title: 'Obtener resultado de los exámenes y entrega de medicamentos',
        text: `¿Desea obtener resultados de los examenes y entrega de medicamentos?`,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: texto_confirmar,
        cancelButtonText: texto_cancelar,
    });

    if (!confirmResult.isConfirmed) {
        return false;
    }

    const evaluacionEessId = document.getElementById('idEvaluacionEESSHidden').value;
    const data = {
        EvaluacionEESSId: evaluacionEessId
    };

    try {
        const response = await fetch(desplieguePost + '/ObtenerResultadosItemEvaluacion', {
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

        Swal.close();

        const evaluacionEessId = document.getElementById('idEvaluacionEESSHidden').value;
        obtenerEvaluacionDetalles(evaluacionEessId);
        mostrarMensaje('Obtener los resultados de los examenes y recojo de medicamentos se ha completado correctamente.', 'success');
    } catch (error) {
        console.error('Error:', error);
        mostrarMensaje('Hubo un problema al obtener los resultados de los examenes y recojo de medicamentos.', 'danger');
    }


}


async function btnAceptarOrden() {  
        
    const idItemAuditoria = document.getElementById('auditoriaItemHidden').value;
    const evaluacionEessId = document.getElementById('idEvaluacionEESSHidden').value;
    const codigoItem = document.getElementById('codigoItemHidden').value; 
    const auditoriaPuntoCarga = document.getElementById('auditoriaPuntoCargaHidden').value;
    const cantidad = document.getElementById('auditoriaCantidad').value;
    const detalleObservacion = document.getElementById('itemAuditoriaDetalleObservacion').value;
    const checkbox = document.getElementById('AuditoriaOrdenSubsanaObs');
    /*const isChecked = checkbox.checked;*/
    const ordenSubsanaObsTexto = document.getElementById('AuditoriaOrdenSubsanaObsTexto').value;
    const correcto = document.getElementById("AuditoriaOrdenCorrecto").checked;
    const auditoriaObservado = document.getElementById("AuditoriaOrdenObservado").checked;

    const MensajesValidacion = document.getElementById('MensajesValidacion');
    const ListaValidaciones = document.getElementById('ListaValidaciones');
    ListaValidaciones.innerHTML = '';
    var valida = false;
    if (!correcto && !auditoriaObservado) {
        valida = true;
        const li = document.createElement('li');
        li.textContent = "Debe seleccionar al menos una opción de auditoría (Correcto u Observado).";
        ListaValidaciones.appendChild(li);
    }

    if (parseFloat(cantidad) <= 0 || isNaN(cantidad) || cantidad.trim().length === 0) {
        valida = true;
        const li = document.createElement('li');
        li.textContent = "La cantidad debe ser mayor que 0.";
        ListaValidaciones.appendChild(li);
    }

    if (auditoriaObservado) {
        if (detalleObservacion.trim().length === 0) {
            valida = true;
            const li = document.createElement('li');
            li.textContent = "Debe registrar el detalle de la observación";
            ListaValidaciones.appendChild(li);
        }
    }

    if (valida) {
        MensajesValidacion.classList.remove('d-none');
        return true;
    }
    else {
        MensajesValidacion.classList.add('d-none');
    }

    const confirmResult = await Swal.fire({
        title: 'Auditar',
        text: `¿Estás seguro de grabar la auditoria?`,
        icon: 'info',
        showCancelButton: true,
        confirmButtonText: texto_confirmar,
        cancelButtonText: texto_cancelar,
    });

    if (!confirmResult.isConfirmed) {
        return false; 
    }

    const data = {
        PuntoCarga: auditoriaPuntoCarga,
        EvaluacionEessId: evaluacionEessId,
        IdItem: idItemAuditoria,
        CodigoItem: codigoItem,
        AuditoriaCantidad: cantidad || null,
        AuditoriaObservacion: detalleObservacion,
        IdEstadoItem: correcto ? 2 : 3, 
        AuditoriaUsuario: 'ADMIN',
        AuditoriaOrdenSubsanaObs: false, // isChecked,
        AuditoriaOrdenSubsanaObsTexto: ordenSubsanaObsTexto
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

        Swal.close();

        obtenerEvaluacionDetalles(evaluacionEessId);
        mostrarMensaje('El registro se ha completado correctamente.', 'success');
    } catch (error) {
        console.error('Error:', error);
        mostrarMensaje('Hubo un problema al realizar la actualización.', 'danger');
    }

}

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

    //filas.forEach(fila => {
    //    const id = fila.getAttribute('data-id');
    //    if (id != null) {
    //        if (id === EvaluacionId.toString()) {
    //            encontrado = true;
    //            const celdaEstado = fila.querySelector('td:nth-child(4)');
    //            celdaEstado.textContent = AuditoriaEstadoId == 2 ? 'Aceptado' : 'Observado';
    //        }
    //    }
    //});
}

async function ActualizarConfirmacionOrden() {
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
        const idEvaluacionHidden = document.getElementById('idEvaluacionHidden').value;
        obtenerEvaluacionDetalles(idEvaluacionHidden);
        mostrarMensaje('La aprobación se ha completado correctamente.', 'success');

    } catch (error) {
        console.error('Error:', error);
        mostrarMensaje('Hubo un problema al realizar la actualización.', 'warning');
    }
}

function mostrarMensaje(mensaje, tipo) {
    var titulo = '¡Advertencia!';
    var icono = 'warning';
    if (tipo == 'warning') {
        titulo = '¡Advertencia!';
        icono = 'warning';
    }
    if (tipo == 'error') {
        titulo = '¡Error!';
        icono = 'error';
    }
    if (tipo == 'success') {
        titulo = '¡Exito!';
        icono = 'success';
    }
    if (tipo == 'info') {
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


const botones = document.querySelectorAll('.responsive-btn');

botones.forEach(btn => {
    const targetId = btn.getAttribute('data-bs-target');
    const collapseEl = document.querySelector(targetId);

    if (collapseEl) {
        collapseEl.addEventListener('show.bs.collapse', () => {
            btn.querySelector('i').textContent = 'expand_less';
        });

        collapseEl.addEventListener('hide.bs.collapse', () => {
            btn.querySelector('i').textContent = 'expand_more';
        });
    }
});

function mostrarDocumento(ruta) {
    // Verificamos si la URL es válida
    if (ruta) {
        // Abrimos una nueva ventana con las opciones de la ruta
        const nuevaVentana = window.open(ruta, '_blank');

        // Si la ventana no se abre (por ejemplo, bloqueada por un popup blocker), mostramos un mensaje
        if (!nuevaVentana) {
            alert('No se pudo abrir el documento en una nueva ventana. Verifique la configuración de su navegador.');
        }
    } else {
        alert('La ruta del documento no es válida.');
    }
}


//function mostrarDocumento(ruta) {
//    const visor = document.getElementById("visorDocumento");
//    const contenedor = document.getElementById("visorDocumentoContainer");

//    if (visor && contenedor) {
//        visor.src = ruta;
//        contenedor.style.display = "block";
//    }
//}


function actualizarCantidadAsignada(isChecked) {
    const inputCantidad = document.getElementById('itemAuditoriaCantidad');
    if (isChecked) {
        inputCantidad.disabled = false;  
    } else {
        inputCantidad.disabled = true;   
    }
}

function AbrirObservacionOrden() {
    const correcto = document.getElementById("AuditoriaOrdenCorrecto").checked;
    const observacionContainer = document.getElementById("AuditoriaObservacionContainer");
    const AuditoriaOrdenSubsanaObsContainer = document.getElementById("AuditoriaOrdenSubsanaObsContainer");
    const auditoriaPuntoCarga = document.getElementById('auditoriaPuntoCargaHidden').value;

    document.getElementById('auditoriaCantidad').value = document.getElementById('cantidadPrescrita').value;
    const auditoriaCantidad = document.getElementById("auditoriaCantidad");
        
    if (correcto) {
        observacionContainer.style.display = "none";
        AuditoriaOrdenSubsanaObsContainer.style.display = "none";
        auditoriaCantidad.setAttribute("readonly", "readonly"); 
    } else {
        observacionContainer.style.display = "block";
        AuditoriaOrdenSubsanaObsContainer.style.display = "block";
        if (auditoriaPuntoCarga === "FARMACIA")
            auditoriaCantidad.removeAttribute("readonly"); 
    }
}

