var despliegue = "/apwAuditoria";
var desplieguePost = "/apwAuditoria";

$(document).ready(function () {
    const fuenteFinanciamientoId = document.getElementById("FuenteFinanciamientoId").value;
    const contenedorParticular = document.getElementById("contenedorParticular");
    const contenedorSis = document.getElementById("contenedorSis");
    if (fuenteFinanciamientoId == 3) {
        contenedorSis.style.display = "block";
    }
    if (fuenteFinanciamientoId == 1) {
        contenedorParticular.style.display = "block";
    }
    const codigoEstado = parseInt(document.getElementById("AuditoriaTriajeCodigoEstadoHidden").value); 
    setEstadoAuditoria(codigoEstado);
});

function setEstadoAuditoria(codigoEstado) {
    const correctoRadio = document.getElementById("AuditoriaTriajeCorrecto");
    const observadoRadio = document.getElementById("AuditoriaTriajeObservado");
    const observacionContainer = document.getElementById("AuditoriaObservacionContainer");
    const triajeSubsanaObsTextoContainer = document.getElementById("AuditoriaTriajeSubsanaObsTextoContainer");
    const triajeSubsanaObsCheckContainer = document.getElementById("AuditoriaTriajeSubsanaObsCheckContainer");

    if (codigoEstado === 2) {
        correctoRadio.checked = true;
        observacionContainer.style.display = "none";
    } else if (codigoEstado === 3) {
        observadoRadio.checked = true;
        observacionContainer.style.display = "block"; 
        triajeSubsanaObsCheckContainer.style.display = "block"; 
        const auditoriaTriajeSubsanaObs = document.getElementById("AuditoriaTriajeSubsanaObs").checked;
        if (auditoriaTriajeSubsanaObs == true) {
            triajeSubsanaObsTextoContainer.style.display = "block"; 
        }
    } else {
        correctoRadio.checked = false;
        observadoRadio.checked = false;
        observacionContainer.style.display = "none";
    }
}


function recargarTiposFinanciamiento() {
    const fuenteFinanciamientoId = document.getElementById("FuenteFinanciamientoId").value;
    const tipoFinanciamientoSelect = document.getElementById("TipoFinanciamientoId");
    const contenedorSis = document.getElementById("contenedorSis");
    const contenedorParticular = document.getElementById("contenedorParticular");

    if (fuenteFinanciamientoId == 1) {
        contenedorParticular.style.display = "block";
    } else {
        contenedorParticular.style.display = "none";
    }

    if (fuenteFinanciamientoId == 3) {
        contenedorSis.style.display = "block";
    } else {
        contenedorSis.style.display = "none";
    }

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
                option.value = tipo.tipo_financiamiento_id; 
                option.text = tipo.descripcion; 
                tipoFinanciamientoSelect.add(option);
            });
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

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

function DevolverNumeroDocumento() {
    const tipoDocumento = document.getElementById('TiposDocumento').value;
    const numeroDocumento = document.getElementById('NumeroDocumento').value;
    return {
        tipoDocumento: tipoDocumento,
        numeroDocumento: numeroDocumento
    };
}

function RecibirValores(valores) {
    document.getElementById("FuenteFinanciamientoId").value = 3;
    const fuenteFinanciamientoId = document.getElementById("FuenteFinanciamientoId").value;
    this.recargarTiposFinanciamiento();
    if (fuenteFinanciamientoId == 3) {
        contenedorSis.classList.remove("d-none");
    }
    document.getElementById('SisAseguradoComponente').value = valores.componente;
    document.getElementById('SisAseguradoDisa').value = valores.disa;
    document.getElementById('SisAseguradoLote').value = valores.lote;
    document.getElementById('SisAseguradoNumero').value = valores.numero;
    document.getElementById('SisAseguradoCorrelativo').value = valores.correlativo;
    document.getElementById('SisAseguradoTipoTabla').value = valores.tipoTabla;
    document.getElementById('SisAseguradoIdentificador').value = valores.identificador;

    document.getElementById('SisAseguradoPlanCobertura').value = valores.idPlan;
    document.getElementById('SisAseguradoGrpPoblacional').value = valores.idGrupoPoblacional;
    document.getElementById('SisAseguradoFechaAfiliacion').value = valores.fecAfiliacion;
    document.getElementById('SisAseguradoTipoSeguro').value = valores.tipoSeguro;
    document.getElementById('SisAseguradoDescTipoSeguro').value = valores.descTipoSeguro;
    document.getElementById('SisAseguradoEstado').value = valores.estado;

}


async function Actualizar() {
    try {
        const atencionId = document.getElementById("idAtencionHidden").value;
        const fechaIngresoAtencion = document.getElementById("FechaIngresoAtencion").value;
        const horaIngresoAtencion = document.getElementById("HoraIngresoAtencion").value;
        const fuenteFinanciamientoId = parseInt(document.getElementById("FuenteFinanciamientoId").value);
        const tipoFinanciamientoId = parseInt(document.getElementById("TipoFinanciamientoId").value);
        const auditoriaTriajeCorrecto = document.getElementById("AuditoriaTriajeCorrecto").checked;
        const auditoriaTriajeObservado = document.getElementById("AuditoriaTriajeObservado").checked;
        const auditoriaTriajeObservacion = document.getElementById("AuditoriaTriajeObservacion").value || '';
        const auditoriaTriajeSubsanaObs = document.getElementById("AuditoriaTriajeSubsanaObs").checked;
        const auditoriaTriajeSubsanaObsTexto = document.getElementById("AuditoriaTriajeSubsanaObsTexto").value || '';

        if (!fechaIngresoAtencion) {
            mostrarModal("modalAdvertencia", "La fecha de ingreso de atención es obligatoria.", 3000);
            return false;
        }
        if (!horaIngresoAtencion) {
            mostrarModal("modalAdvertencia", "La hora de ingreso de atención es obligatoria.", 3000);
            return false;
        }
        if (isNaN(fuenteFinanciamientoId) || fuenteFinanciamientoId <= 0) {
            mostrarModal("modalAdvertencia", "Debe seleccionar una fuente de financiamiento válida.", 3000);
            return false;
        }
        if (isNaN(tipoFinanciamientoId) || tipoFinanciamientoId <= 0) {
            mostrarModal("modalAdvertencia", "Debe seleccionar un tipo de financiamiento válido.", 3000);
            return false;
        }
        if (!auditoriaTriajeCorrecto && !auditoriaTriajeObservado) {
            mostrarModal("modalAdvertencia", "Debe seleccionar al menos una opción de auditoría (Triaje Correcto u Observado).", 3000);
            return false;
        }
        if (auditoriaTriajeObservado && auditoriaTriajeObservacion.trim() === '') {
            mostrarModal("modalAdvertencia", "Debe proporcionar una observación si Triaje Observado está seleccionado.", 3000);
            return false;
        }

        if (!auditoriaTriajeObservado) {
            if (fuenteFinanciamientoId == 1) {
                const ParticularBoletaApertura = document.getElementById("ParticularBoletaApertura").value.trim();
                if (ParticularBoletaApertura.length === 0) {
                    mostrarModal("modalAdvertencia", "El Nro Comprobante Pago es obligatorio.", 3000);
                    return false;
                }
            }
            if (fuenteFinanciamientoId == 3) {
                const componente = document.getElementById("SisAseguradoComponente").value.trim();
                const disa = document.getElementById("SisAseguradoDisa").value.trim();
                const lote = document.getElementById("SisAseguradoLote").value.trim();
                const numero = document.getElementById("SisAseguradoNumero").value.trim();
                const correlativo = document.getElementById("SisAseguradoCorrelativo").value.trim();
                const tipoTabla = document.getElementById("SisAseguradoTipoTabla").value.trim();
                const identificador = document.getElementById("SisAseguradoIdentificador").value.trim();

                if (isNaN(componente) || componente.length !== 1) {
                    mostrarModal("modalAdvertencia", "El campo 'Componente' debe ser numérico y tener exactamente 1 dígito.", 3000);
                    return false;
                }

                if (isNaN(disa) || disa.length === 0 || disa.length > 3) {
                    mostrarModal("modalAdvertencia", "El campo 'Disa' debe ser numérico y tener un máximo de 3 dígitos.", 3000);
                    return false;
                }
                if (isNaN(lote) || lote.length < 1 || lote.length > 2) {
                    mostrarModal("modalAdvertencia", "El campo 'Lote' debe ser numérico y tener entre 1 y 2 dígitos.", 3000);
                    return false;
                }
                if (isNaN(numero) || numero.length < 8 || numero.length > 9) {
                    mostrarModal("modalAdvertencia", "El campo 'Numero' debe ser numérico y tener entre 8 y 9 dígitos.", 3000);
                    return false;
                }
                if (correlativo.length > 2) {
                    mostrarModal("modalAdvertencia", "El campo 'Correlativo' debe tener un máximo de 2 dígitos.", 3000);
                    return false;
                }
                if (correlativo.length > 0) {
                    if (isNaN(correlativo)) {
                        mostrarModal("modalAdvertencia", "El campo 'Correlativo' debe ser numérico.", 3000);
                        return false;
                    }
                }
                if (isNaN(tipoTabla) || tipoTabla.length === 0 || tipoTabla.length > 1) {
                    mostrarModal("modalAdvertencia", "El campo 'Tipo Tabla' debe ser numérico y tener un máximo de 1 dígito.", 3000);
                    return false;
                }
                if (isNaN(identificador) || identificador.length === 0 || identificador.length > 10) {
                    mostrarModal("modalAdvertencia", "El campo 'Identificador' debe ser numérico y tener un máximo de 10 dígitos.", 3000);
                    return false;
                }
            }
        }


        const data = {
            AtencionId: atencionId,
            FechaIngresoAtencion: fechaIngresoAtencion,
            HoraIngresoAtencion: horaIngresoAtencion,
            FuenteFinanciamientoId: fuenteFinanciamientoId,
            TipoFinanciamientoId: tipoFinanciamientoId,
            SisAseguradoComponente: document.getElementById("SisAseguradoComponente").value,
            SisAseguradoDisa: document.getElementById("SisAseguradoDisa").value,
            SisAseguradoLote: document.getElementById("SisAseguradoLote").value,
            SisAseguradoNumero: document.getElementById("SisAseguradoNumero").value,
            SisAseguradoCorrelativo: document.getElementById("SisAseguradoCorrelativo").value,
            SisAseguradoTipoTabla: document.getElementById("SisAseguradoTipoTabla").value,
            SisAseguradoIdentificador: document.getElementById("SisAseguradoIdentificador").value,

            SisAseguradoPlanCobertura: document.getElementById("SisAseguradoPlanCobertura").value,
            SisAseguradoGrpPoblacional: document.getElementById("SisAseguradoGrpPoblacional").value,
            SisAseguradoFechaAfiliacion: document.getElementById("SisAseguradoFechaAfiliacion").value,
            SisAseguradoTipoSeguro: document.getElementById("SisAseguradoTipoSeguro").value,
            SisAseguradoDescTipoSeguro: document.getElementById("SisAseguradoDescTipoSeguro").value,
            SisAseguradoEstado: document.getElementById("SisAseguradoEstado").value,

            ParticularBoletaApertura: document.getElementById("ParticularBoletaApertura").value,
            ParticularConceptoBoleta: document.getElementById("ParticularConceptoBoleta").value,
            AuditoriaObservacion: '',
            AuditoriaTriajeCodigoEstado: auditoriaTriajeCorrecto ? 2 : 3,
            AuditoriaUsuario: 'FCACHAY',
            AuditoriaTriajeObservacion: auditoriaTriajeObservacion,
            AuditoriaTriajeSubsanaObs: auditoriaTriajeSubsanaObs,
            AuditoriaTriajeSubsanaObsTexto: auditoriaTriajeSubsanaObsTexto
        };

        const response = await fetch(this.desplieguePost + '/ActualizarTriaje', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        // 🔹 Validar si el token no es válido (401 Unauthorized)
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
            var mensaje = `Se guardó correctamente. Se registró la cuenta ${result.cuenta_atencion_id}`;
            if (!auditoriaTriajeCorrecto) {
                if (auditoriaTriajeSubsanaObs)
                    mensaje = `Se subsano correctamente la observación. Se registró la cuenta ${result.cuenta_atencion_id}`;
                else
                    mensaje = `Se guardó correctamente la observación del triaje`;
            }


            mostrarModal("modalGuardadoExitoso", mensaje, 3000);
            return true;
        } else {
            mostrarModal("modalAdvertencia", "Hubo un error al guardar.", 3000);
            return false;
        }
    } catch (error) {
        console.error('Error en Actualizar:', error);
        mostrarModal("modalError", "Ocurrió un error al intentar guardar la atención.", 3000);
        return false;
    }
}


function validarLongitud(input, maxLength) {
    if (input.value.length > maxLength) {
        input.value = input.value.slice(0, maxLength);
    }
}

function AbrirObservacionTriaje() {
    const correcto = document.getElementById("AuditoriaTriajeCorrecto").checked;
    const observacionContainer = document.getElementById("AuditoriaObservacionContainer");

    if (correcto) {
        observacionContainer.style.display = "none";
    } else {
        observacionContainer.style.display = "block";        
    }
}


function btnActualizar() {
    const idAtencionHidden = document.getElementById('idAtencionHidden').value;
    // Recopilar los datos del formulario
    const data = {
        atencionId: idAtencionHidden,
        auditoriaUsuario: 'ADMIN'
    };

    fetch('AtencionCEAuditoria', {
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

function AbrirModalSIS() {
    var valorDNI = document.getElementById('NumeroDocumento').value;
    var valorHC = document.getElementById('HistoriaClinica').value;

    var ceSelected = document.getElementById('TiposDocumento').value == 3 ? 'selected' : '';
    var dniSelected = document.getElementById('TiposDocumento').value == 3 ? '' : 'selected';
    
    Swal.fire({
        title: '<div class="bg-banner text-white d-flex align-items-center justify-content-center py-2 px-3"><h3 class="m-0">Consultar afiliación SIS</h3></div>',
        html: `

        <div class="row">
            <div class="col-md-3">
                <div class="position-relative border rounded p-3 mt-3" style="border-color: #6c757d; border-width: 1px; border-style: dashed;">
                    <span class="position-absolute top-0 start-0 translate-middle-y bg-white px-2" style="font-weight: bold; color: #4a90e2;">
                        <i class="material-icons me-2">search</i> BUSQUEDA
                    </span>
                    <div class="row m-2">

                           <div class="mb-3">
                                <div class="form-check d-flex align-items-center">
                                    <input class="form-check-input" type="radio" name="SISBuscarAfiliado" id="SISBuscarPorDNI" checked onchange="seleccionarBusquedaSIS()">
                                    <label class="form-check-label ms-2" for="SISBuscarPorDNI">Nro DNI</label>
                                </div>

                            </div>
                            <div class="mb-3" id="SeccionBuscarPorDNI">
                                <label for="SISBuscarPorTipoDocumento" class="col-form-label ms-2">Tipo documento:</label>
                                <select class="form-select" id="SISBuscarPorTipoDocumento" onchange="ActualizarTipoDocumentoSIS()" aria-disabled="true" disabled>
                                    <option value="dni" ${dniSelected}'>DNI</option>
                                    <option value="ce" ${ceSelected}>Carné de extranjería</option>
                                </select>
                            </div>
                            <div class="mb-3" id="dniNumberSection">
                                <label for="numeroDocumentoSIS" class="col-form-label">Nro documento:</label>
                                <input type="text" class="form-control" id="numeroDocumentoSIS" value="${valorDNI}" placeholder="Ingrese número" disabled>
                            </div>
                            <div class="mb-3">

                                <div class="form-check d-flex align-items-center">
                                    <input class="form-check-input" type="radio" name="SISBuscarAfiliado" id="BuscarPorHC" onchange="seleccionarBusquedaSIS()">
                                    <label class="form-check-label ms-2" for="BuscarPorHC">Nro Historia</label>
                                </div>                                

                            </div>                           
                            <div class="mb-3" id="SeccionBuscarPorHC" style="display: none;">
                                <label for="SISBuscarPorHC" class="col-form-label">Nro Afiliación:</label>
                                <input type="text" class="form-control" id="SISBuscarPorHC" value="${valorHC}"  placeholder="Ingrese Historia Clinica">
                            </div>
                            <div class="d-flex justify-content-between">
                                <button type="button" class="btn btn-primary w-100" id="btnBuscarPorHC" onclick="BuscarSIS()">
                                    <span class="material-icons">search</span> Buscar
                                </button>
                            </div>

                    </div>

                </div>
            </div>
            <div class="col-md-9">
                 <div class="position-relative border rounded p-3 mt-3" style="border-color: #6c757d; border-width: 1px; border-style: dashed;">
                    <span class="position-absolute top-0 start-0 translate-middle-y bg-white px-2" style="font-weight: bold; color: #4a90e2;">
                        <i class="material-icons me-2">search</i> RESULTADO
                    </span>

                    <div class="row m-2" id="textResultadoBusquedaSIS">
                        <div class="alert alert-success" role="alert">                         
                          <p id="mensajeResultadoSIS"></p>
                        </div>
                    </div>
                    <div class="row m-2" id="resultadoBusquedaSIS">
                        <div class="col-sm-3 mb-2">
                            <label for="documentResult" class="form-label">Nro de documento</label>
                            <input type="text" class="form-control" id="SisDocumento" name="" readonly>
                        </div>
                        <div class="col-sm-3 mb-2">
                            <label for="SisFechaNacimiento" class="form-label">Fec. de nacimiento</label>
                            <input type="text" class="form-control" id="SisFechaNacimiento" readonly>
                        </div>
                        <div class="col-sm-3 mb-2">
                            <label for="SisContrato" class="form-label">Contrato</label>
                            <input type="text" class="form-control" id="SisContrato" name="" readonly>
                        </div>
                        <div class="col-sm-3 mb-2">
                            <label for="SisFechaAfiliacion" class="form-label">Fec. de afiliación</label>
                            <input type="text" class="form-control" id="SisFechaAfiliacion" readonly>
                        </div>
                        <div class="col-sm-6 mb-2">
                            <label for="nameResult" class="form-label">Nombres</label>
                            <input type="text" class="form-control" id="SisNombre" readonly>
                        </div>                     
                        <div class="col-sm-6 mb-2">
                            <label for="insuranceType" class="form-label">Tipo de seguro</label>
                            <input type="text" class="form-control" id="SisTipoSeguro" readonly>
                        </div>
                         <div class="col-sm-3 mb-2">
                            <label for="SisDisa" class="form-label">Disa</label>
                            <input type="text" class="form-control" id="SisDisa" readonly>
                        </div>
                         <div class="col-sm-3 mb-2">
                            <label for="SisLote" class="form-label">Lote</label>
                            <input type="text" class="form-control" id="SisLote" readonly>
                        </div>
                        <div class="col-sm-3 mb-2">
                            <label for="SisNroContrato" class="form-label">Nro de contrato</label>
                            <input type="text" class="form-control" id="SisNroContrato" name="" readonly>
                        </div>
                        <div class="col-sm-3 mb-2">
                            <label for="SisEstado" class="form-label">Estado</label>
                            <input type="text" class="form-control bg-warning text-primary" id="SisEstado" readonly>
                        </div>                       
                    </div>          
                    
                </div>

                <input type="hidden" id="SisAseguradoIdErrorHidden" name="SisAseguradoIdErrorHidden">
                <input type="hidden" id="SisAseguradoComponenteHidden" name="SisAseguradoComponenteHidden">
                <input type="hidden" id="SisAseguradoDisaHidden" name="SisAseguradoDisaHidden">
                <input type="hidden" id="SisAseguradoLoteHidden" name="SisAseguradoLoteHidden">
                <input type="hidden" id="SisAseguradoNumeroHidden" name="SisAseguradoNumeroHidden">
                <input type="hidden" id="SisAseguradoCorrelativoHidden" name="SisAseguradoCorrelativoHidden">
                <input type="hidden" id="SisAseguradoTipoTablaHidden" name="SisAseguradoTipoTablaHidden">
                <input type="hidden" id="SisAseguradoIdentificador" name="SisAseguradoIdentificador">

                <input type="hidden" id="SisAseguradoPlanCobertura" name="SisAseguradoPlanCobertura">
                <input type="hidden" id="SisAseguradoGrpPoblacional" name="SisAseguradoGrpPoblacional">
                <input type="hidden" id="SisAseguradoFechaAfiliacion" name="SisAseguradoFechaAfiliacion">
                <input type="hidden" id="SisAseguradoTipoSeguro" name="SisAseguradoTipoSeguro">
                <input type="hidden" id="SisAseguradoDescTipoSeguro" name="SisAseguradoDescTipoSeguro">
                <input type="hidden" id="SisAseguradoEstado" name="SisAseguradoEstado">

            </div>
        </div>
		<div class="row justify-content-center">
            <div class="col-md-6">

                <div class="d-flex gap-2 justify-content-center">
                    <button type="button" id="btnGuardarSIS" class="btn btn-primary d-flex align-items-center justify-content-center gap-2 responsive-btn" data-bs-toggle="tooltip" data-bs-placement="top" title="Guardar">
                        <i class="material-icons">save</i> Guardar
                    </button>
                    <button type="button" id="btnSalirSIS" class="btn btn-secondary d-flex align-items-center justify-content-center gap-2 responsive-btn" data-bs-dismiss="modal" data-bs-toggle="tooltip" data-bs-placement="top" title="Salir">
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
            document.getElementById('btnSalirSIS').addEventListener('click', () => {
                Swal.close();
            });

            document.getElementById('btnGuardarSIS').addEventListener('click', () => {
                /*Swal.fire('Acción confirmada', '', 'success');*/
                this.btnAceptarSIS();
            });
        }
    });
}

function AbrirModalParticular() {
    var valorHC = document.getElementById('HistoriaClinica').value;

    Swal.fire({
        title: '<div class="bg-banner text-white d-flex align-items-center justify-content-center py-2 px-3"><h3 class="m-0"> Consultar Comprobante Pago</h3></div>',
        html: `

        <div class="row m-2">
            <div class="col-md-3">
                <div class="position-relative border rounded p-3 mt-3" style="border-color: #6c757d; border-width: 1px; border-style: dashed;">
                    <span class="position-absolute top-0 start-0 translate-middle-y bg-white px-2" style="font-weight: bold; color: #4a90e2;">
                        <i class="material-icons me-2">search</i> BUSQUEDA
                    </span>                    
                     <div class="mb-3">
                        <label for="numeroHistoria" class="col-form-label">Nro Historia:</label>
                        <input type="text" class="form-control" id="numeroHistoria" value="${valorHC}" placeholder="Ingrese HC">
                    </div>
                      <div class="mb-3">
                        <label for="nombrePaciente" class="col-form-label">Nomb. Paciente Comprobante:</label>
                        <input type="text" class="form-control" id="nombrePaciente" placeholder="Ingrese nombre paciente">
                    </div>
                    <div class="mb-3">
                        <label for="numeroComprobantePago" class="col-form-label">Nro. Comprobante:</label>
                        <input type="text" class="form-control" id="numeroComprobantePago" placeholder="Ingrese Comprobante">
                    </div>
                    <div class="mb-3" id="listaComprobantes">
                        <ul class="list-group" id="comprobanteList">                            
                        </ul>
                    </div>
                    <div class="d-flex justify-content-between">
                        <button type="button" class="btn btn-primary w-100" id="btnBuscarComprobante" onclick="BuscarComprobante()">
                            <span class="material-icons">search</span> Buscar
                        </button>
                    </div>
                </div>
            </div>
            <div class="col-md-9">
               <div class="position-relative border rounded p-3 mt-3" style="border-color: #6c757d; border-width: 1px; border-style: dashed;">
                    <span class="position-absolute top-0 start-0 translate-middle-y bg-white px-2" style="font-weight: bold; color: #4a90e2;">
                        <i class="material-icons me-2">search</i> RESULTADO
                    </span>

                    <div class="row m-2" id="textResultadoBusquedaComprobante">
                        <div class="alert alert-success" role="alert">                         
                          <p id="mensajeResultadoComprobante"></p>
                        </div>
                    </div>
                    <div class="row  m-2" id="resultadoBusquedaComprobante">
                        <div class="col-sm-3 mb-2">
                            <label style="display: none;" for="ParticularNroBoletaRes" class="form-label">Nro de Comprobante</label>
                            <input type="hidden" class="form-control" id="ParticularNroBoletaRes" readonly>
                        </div>                     
                        <div class="col-sm-9 mb-2">
                            <label style="display: none;" for="ParticularConceptoBoletaRes" class="form-label">Concepto Comprobante</label>
                            <input type="hidden" class="form-control" id="ParticularConceptoBoletaRes" readonly>
                        </div>                     
                       
                    </div>
                    
                </div>

            </div>
        </div>
		<div class="row justify-content-center">
            <div class="col-md-6">

                <div class="d-flex gap-2 justify-content-center">                  
                    <button type="button" id="btnSalirComprobante" class="btn btn-secondary d-flex align-items-center justify-content-center gap-2 responsive-btn" data-bs-dismiss="modal" data-bs-toggle="tooltip" data-bs-placement="top" title="Salir">
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
            document.getElementById('btnSalirComprobante').addEventListener('click', () => {
                Swal.close();
            });

            //document.getElementById('btnGuardarComprobante').addEventListener('click', () => {
            //    this.btnGuardarComprobante();
            //});
        }
    });
}


function ActualizarTipoDocumentoSIS() {
    const documentType = document.getElementById('tipoDocumentoSIS').value;
    const documentNumberInput = document.getElementById('numeroDocumentoSIS');

    documentNumberInput.value = '';
    if (documentTypes[documentType]) {
        const length = documentTypes[documentType].length;
        if (typeof length === "object") {
            documentNumberInput.setAttribute("maxlength", length.max);
            documentNumberInput.placeholder = `Ingrese un número entre ${length.min} y ${length.max} dígitos`;
        } else {
            documentNumberInput.setAttribute("maxlength", length);
            documentNumberInput.placeholder = `Ingrese un número de ${length} dígitos`;
        }
        documentNumberInput.setAttribute("type", "number");
    }
}

function seleccionarBusquedaSIS() {
    const searchByDNI = document.getElementById('SISBuscarPorDNI').checked;
    const dniSection = document.getElementById('SeccionBuscarPorDNI');
    const dniNumberSection = document.getElementById('dniNumberSection');
    const affiliationSection = document.getElementById('SeccionBuscarPorHC');

    if (searchByDNI) {
        dniSection.style.display = 'block';
        dniNumberSection.style.display = 'block';
        affiliationSection.style.display = 'none';
    } else {
        dniSection.style.display = 'none';
        dniNumberSection.style.display = 'none';
        affiliationSection.style.display = 'block';
    }
}

function BuscarSIS() {
    var s_tipobus = document.getElementById('SISBuscarPorTipoDocumento').value;
    var s_numero = document.getElementById('numeroDocumentoSIS').value;
    const searchByDNI = document.getElementById('SISBuscarPorDNI').checked;
    if (!searchByDNI) {     
        s_tipobus = 'hc';
        s_numero = document.getElementById('SISBuscarPorHC');
    }        
    const url = `${this.despliegue}/Atencion/BuscarAfiliadoSIS?s_tipobus=${s_tipobus}&s_numero=${s_numero}`;
    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error("Error al buscar afiliado SIS");
            }
            return response.json();
        })
        .then(data => {
            if (data) {
                document.getElementById('mensajeResultadoSIS').innerHTML = data.resultado;
                document.getElementById("SisAseguradoIdErrorHidden").value = data.idError;

                if (data.idError == '0') {
                    document.getElementById("SisDocumento").value = data.nroDocumento || '';
                    document.getElementById("SisNombre").value = `${data.nombres || ''} ${data.apePaterno || ''} ${data.apeMaterno || ''}`;
                    document.getElementById("SisFechaNacimiento").value = data.fecNacimiento || '';
                    document.getElementById("SisTipoSeguro").value = data.descTipoSeguro || '';
                    document.getElementById("SisContrato").value = data.contrato || '';
                    document.getElementById("SisFechaAfiliacion").value = data.fecAfiliacion || '';
                    document.getElementById("SisAseguradoComponenteHidden").value = data.regimen || '';
                    document.getElementById("SisAseguradoDisaHidden").value = data.disa || '';
                    document.getElementById("SisAseguradoLoteHidden").value = data.tipoFormato || '';
                    document.getElementById("SisAseguradoNumeroHidden").value = data.nroContrato || '';
                    document.getElementById("SisAseguradoCorrelativoHidden").value = data.correlativo || '';
                    document.getElementById("SisAseguradoTipoTablaHidden").value = data.tabla || '';
                    document.getElementById("SisAseguradoIdentificador").value = data.idNumReg || '';

                    document.getElementById("SisAseguradoPlanCobertura").value = data.idPlan || '';
                    document.getElementById("SisAseguradoGrpPoblacional").value = data.idGrupoPoblacional || '';
                    document.getElementById("SisAseguradoFechaAfiliacion").value = data.fecAfiliacion || '';
                    document.getElementById("SisAseguradoTipoSeguro").value = data.tipoSeguro || '';
                    document.getElementById("SisAseguradoDescTipoSeguro").value = data.descTipoSeguro || '';
                    document.getElementById("SisAseguradoEstado").value = data.estado || '';

                    console.log(data.descTipoSeguro);
                    console.log(document.getElementById("SisAseguradoDescTipoSeguro").value);

                    document.getElementById("SisDisa").value = data.disa || '';
                    document.getElementById("SisLote").value = data.tipoFormato || '';
                    document.getElementById("SisNroContrato").value = data.nroContrato || '';
                    document.getElementById("SisEstado").value = data.estado || '';
                }                         
            } else {
                console.error("No se encontraron datos para el afiliado");
            }
        })
        .catch(error => {
            console.error("Error:", error);
        });
}

//function BuscarComprobante() {    
//    var s_numero = document.getElementById('numeroComprobantePago').value;  
//    document.getElementById('mensajeResultadoComprobante').innerHTML = "Comprobante de pago encontrado";
//    document.getElementById("ParticularNroBoletaRes").value = s_numero;
//    document.getElementById("ParticularConceptoBoletaRes").value = "Pago por concepto de atención particular";
//}


function btnAceptarSIS() {
    if (document.getElementById("SisAseguradoIdErrorHidden").value != '0') {
        Swal.fire({
            title: '¡Advertencia!',
            text: 'NO SE ENCONTRO AFILIACION PARA EL PACIENTE CONSULTADO',
            icon: 'warning',
            confirmButtonText: 'OK'
        });
    }
    else {
        const valoresParaIframe = {
            componente: document.getElementById('SisAseguradoComponenteHidden').value,
            disa: document.getElementById('SisAseguradoDisaHidden').value,
            lote: document.getElementById('SisAseguradoLoteHidden').value,
            numero: document.getElementById('SisAseguradoNumeroHidden').value,
            correlativo: document.getElementById('SisAseguradoCorrelativoHidden').value,
            tipoTabla: document.getElementById('SisAseguradoTipoTablaHidden').value,
            identificador: document.getElementById('SisAseguradoIdentificador').value, 

            idPlan: document.getElementById('SisAseguradoPlanCobertura').value, 
            idGrupoPoblacional: document.getElementById('SisAseguradoGrpPoblacional').value, 
            fecAfiliacion: document.getElementById('SisAseguradoFechaAfiliacion').value, 
            tipoSeguro: document.getElementById('SisAseguradoTipoSeguro').value, 
            descTipoSeguro: document.getElementById('SisAseguradoDescTipoSeguro').value, 
            estado: document.getElementById('SisAseguradoEstado').value

        };

        console.log(valoresParaIframe);
        this.RecibirValores(valoresParaIframe);
        Swal.close();
    }
}

//function btnGuardarComprobante() {
//    document.getElementById('ParticularBoletaApertura').value = document.getElementById('ParticularNroBoletaRes').value;
//    document.getElementById('ParticularConceptoBoleta').value = document.getElementById('ParticularConceptoBoletaRes').value;
//    Swal.close();
//}

function cerrarModalSIS() {
    Swal.close();
}

function SubsanarObservacion() {
    const checkbox = document.getElementById('AuditoriaTriajeSubsanaObs');
    const container = document.getElementById('AuditoriaTriajeSubsanaObsTextoContainer');
    if (checkbox.checked) {
        container.style.display = 'block';
    } else {
        container.style.display = 'none';
    }
}


function BuscarComprobante() {
    const nroHistoria = document.getElementById('numeroHistoria').value.trim();
    const nroBoleta = document.getElementById('numeroComprobantePago').value.trim();
    const nombPaciente = document.getElementById('nombrePaciente').value.trim();
    

    const resultadoBusqueda = document.getElementById('resultadoBusquedaComprobante');
    const mensajeResultado = document.getElementById('mensajeResultadoComprobante');

    resultadoBusqueda.innerHTML = '';
    mensajeResultado.textContent = '';

    if (!nroHistoria) {
        mensajeResultado.textContent = "Por favor, ingrese un número de historia.";
        return;
    }

    const url = `${this.despliegue}/Atencion/detalleBoletaAtencion?nroHistoria=${nroHistoria}&nroBoleta=${nroBoleta}&nombPaciente=${nombPaciente}`;
    fetch(url)
        .then(response => {
            console.log(response);
            if (!response.ok) {
                throw new Error("Error al buscar afiliado SIS");
            }
            return response.json();
        })
        .then(result => {
            const data = result.data;
            if (data && data.length > 0) {
                mensajeResultado.textContent = "Resultados encontrados:";
                RenderizarListaComprobantes(data, resultadoBusqueda);
            } else {
                mensajeResultado.textContent = "No se encontraron comprobantes con el número ingresado.";
            }
        })
        .catch(error => {
            console.error("Error:", error);
            mensajeResultado.textContent = "Ocurrió un error al buscar los comprobantes.";
        });
}

function RenderizarListaComprobantes(lista, contenedor) {
    lista.forEach(comprobante => {
        const item = document.createElement('button'); 
        item.classList.add('mb-2', 'p-2', 'border', 'rounded', 'bg-light', 'w-100', 'text-start', 'd-flex', 'flex-wrap');
        item.style.textAlign = 'left'; 

        item.innerHTML = `
            <div style="width: 50%; padding-right: 10px;">
                <strong>Número:</strong> ${comprobante.cmbol_nume}
            </div>
            <div style="width: 50%; padding-right: 10px;">
                <strong>HC:</strong> ${comprobante.hc}
            </div>
            <div style="width: 50%; padding-left: 10px;">
                <strong>Fecha:</strong> ${new Date(comprobante.cmbol_femi).toLocaleDateString()}
            </div>
            <div style="width: 50%; padding-right: 10px;">
                <strong>Total:</strong> S/ ${comprobante.cmbol_totp.toFixed(2)}
            </div>
            <div style="width: 50%; padding-left: 10px;">
                <strong>Pagante:</strong> ${comprobante.cmbol_nomb}
            </div>
            <div style="width: 50%; padding-right: 10px;">
                <strong>Concepto:</strong> ${comprobante.cmta_desc}
            </div>
        `;

        item.addEventListener('click', () => {
            document.getElementById('ParticularBoletaApertura').value = comprobante.cmbol_nume;
            document.getElementById('ParticularConceptoBoleta').value = comprobante.cmta_desc;
            Swal.close();
        });

        contenedor.appendChild(item);
    });
}



//function RenderizarListaComprobantes(lista, contenedor) {
//    lista.forEach(comprobante => {
//        const item = document.createElement('div');
//        item.classList.add('mb-2', 'p-2', 'border', 'rounded', 'bg-light');

//        item.innerHTML = `
//            <div><strong>Número:</strong> ${comprobante.cmbol_nume}</div>
//            <div><strong>Fecha:</strong> ${new Date(comprobante.cmbol_femi).toLocaleDateString()}</div>
//            <div><strong>Total:</strong> S/ ${comprobante.cmbol_totp.toFixed(2)}</div>
//            <div><strong>Concepto:</strong> ${comprobante.cmta_desc}</div>
//        `;

//        contenedor.appendChild(item);
//    });
//}








// Seccion para la version 2, agregar multiples observaciones
function abrirModalObservacion(index) {
    if (index === 0) {
        document.getElementById('observacionModalLabel').innerText = "Registrar Observación";
        document.getElementById('observacionInput').value = "";
        document.getElementById('estadoInput').value = "Pendiente";
        document.getElementById('observacionId').value = "0";
    } else {
        const fila = document.querySelector(`tr[data-index='${index}']`);
        const descripcion = fila.cells[2].innerText; 
        const estado = fila.cells[3].innerText;

        document.getElementById('observacionModalLabel').innerText = "Editar Observación";
        document.getElementById('observacionInput').value = descripcion;
        document.getElementById('estadoInput').value = estado;
        document.getElementById('observacionId').value = index;
    }

    const modal = new bootstrap.Modal(document.getElementById('observacionModal'));
    modal.show();
}

function guardarObservacion() {
    const index = parseInt(document.getElementById('observacionId').value);
    const descripcion = document.getElementById('observacionInput').value;
    const estado = document.getElementById('estadoInput').value;

    if (index === 0) {
        // Nuevo registro
        const nuevaFila = `
                <tr data-index="${getNextIndex()}">
                   <td class="text-center small campo-grilla p-0">
                        <div class="d-flex justify-content-start">
                            <button type="button" class="btn btn-success btn-sm d-flex w-100 align-items-center justify-content-center p-0 ms-1" style="width: 40px; height: 40px;" onclick="abrirModalObservacion(${getNextIndex()})" data-bs-toggle="tooltip" data-bs-placement="top" title="Subsanar">
                                <i class="material-icons">check_circle</i>
                            </button>
                        </div>
                    </td>                    
                    <td class="small campo-grilla">${getNextIndex()}</td>
                    <td class="small campo-grilla">${descripcion}</td>
                    <td class="small campo-grilla">${estado}</td>
                    <td class="text-center small campo-grilla p-0">
                        <div class="d-flex justify-content-start">
                            <button type="button" class="btn btn-primary btn-sm d-flex w-50 align-items-center justify-content-center p-0" style="width: 40px; height: 40px;" onclick="abrirModalObservacion(${getNextIndex()})" data-bs-toggle="tooltip" data-bs-placement="top" title="Editar">
                                <i class="material-icons">edit</i>
                            </button>
                            <button type="button" class="btn btn-danger btn-sm d-flex w-50 align-items-center justify-content-center p-0 ms-1" style="width: 40px; height: 40px;" onclick="eliminarObservacion(${getNextIndex()})" data-bs-toggle="tooltip" data-bs-placement="top" title="Eliminar">
                                <i class="material-icons">delete</i>
                            </button>
                        </div>
                    </td>
                   
                </tr>
            `;
        document.getElementById('observacionesTableBody').insertAdjacentHTML('beforeend', nuevaFila);
    } else {
        const fila = document.querySelector(`tr[data-index='${index}']`);
        fila.cells[2].innerText = descripcion;
        fila.cells[3].innerText = estado;
    }

    bootstrap.Modal.getInstance(document.getElementById('observacionModal')).hide();
    regenerarIndices();
}

function eliminarObservacion(index) {
    const fila = document.querySelector(`tr[data-index='${index}']`);
    fila.remove();
    regenerarIndices();
}

function getNextIndex() {
    const filas = document.querySelectorAll('#observacionesTableBody tr');
    return filas.length + 1;
}

function regenerarIndices() {
    const filas = document.querySelectorAll('#observacionesTableBody tr');
    filas.forEach((fila, idx) => {
        fila.setAttribute('data-index', idx + 1);
        fila.cells[1].innerText = idx + 1;
        fila.querySelector('button.btn-primary').setAttribute('onclick', `abrirModalObservacion(${idx + 1})`);
        fila.querySelector('button.btn-danger').setAttribute('onclick', `eliminarObservacion(${idx + 1})`);
    });
}


