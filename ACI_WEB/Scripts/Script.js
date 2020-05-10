$(document).ready(function () {
    // Javascript method's body can be found in assets/js/demos.js
    //demo.initDashboardPageCharts();

    $('li a').click(function (e) {
        //e.preventDefault();
        $('a').removeClass('active');
        $(this).addClass('active');
    });

    var mensaje = getParameterByName('msg');

    if (mensaje != "") {
        switch (mensaje){
            case 'atendido': 
                nowuiDashboard.showNotification('top', 'right', "Se atendió correctamente el ticket.");
                break;
            case 'derivado':
                nowuiDashboard.showNotification('top', 'right', "Se derivó correctamente el ticket.");
                break;
            default:
                break;
        }
    }

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    $.ajax({
        type: 'GET',
        url: '/Responsable/ListarResponsables/',
        success: function (result) {
            var a = '<option value="-1">Seleccione un Responsable</option>';
            for (var i = 0; i < result.length; i++) {
                a += '<option value="' + result[i].codresponsable + '">' + result[i].responsable1 + '</option>';
            }
            $('#comboboxResponsable').html(a);
        }
    });



    $('#comboboxEquipo').on('change', function () {
        
        var codequipo = $('#comboboxEquipo option:selected').val();

        if (codequipo == 2) {
            $('#subtipo').show();
        } else {
            $('#subtipo').hide();
        }

        $.ajax({
            type: 'GET',
            data: { codequipo: codequipo },
            url: '/Aplicativo/ListarAplicativos/',//'@Url.Action("ListarAplicativos","Ticket")',
            success: function (result) {
                var a = '<option value="-1">Seleccione un Aplicativo o Cartera</option>';
                for (var i = 0; i < result.length; i++) {
                    a += '<option value="' + result[i].codaplicativo + '">' + result[i].aplicativo1 + '</option>';
                }
                $('#comboboxAplicativo').html(a);
            },
            complete: function (result) {
                $.ajax({
                    type: 'GET',
                    data: { codequipo: codequipo },
                    url: '/Tipo/ListarTipos/',//'@Url.Action("ListarAplicativos","Ticket")',
                    success: function (result) {
                        var a = '<option value="-1">Seleccione un Tipo de Ticket</option>';
                        for (var i = 0; i < result.length; i++) {
                            a += '<option value="' + result[i].codtipo + '">' + result[i].tipo1 + '</option>';
                        }
                        $('#comboboxTipo').html(a);
                    }
                });
            }
        });
    });

    $('#comboboxTipo').on('change', function () {
        var codtipo = $('#comboboxTipo option:selected').val();

        if (codtipo == "4") {
            $('#subtipo').hide();
            $('#matricula_modelo').show();
        } else {
                       
        $.ajax({
            type: 'GET',
            data: { codtipo: codtipo },
            url: '/SubTipo/ListarSubTipos/',//'@Url.Action("ListarSubTipos", "Ticket")',
            success: function (result) {
                var a = '<option value="-1">Seleccione un Subtipo</option>';
                for (var i = 0; i < result.length; i++) {
                    a += '<option value="' + result[i].codsubtipo + '">' + result[i].subtipo1 + '</option>';
                }
                $('#comboboxSubTipo').html(a);
            }
            });

            $('#subtipo').show();
            $('#matricula_modelo').hide();
        }
    });

    $('#comboboxTipoDerivacion').on('change', function () {

        var codequipo = $('#codequipo').val();
        var codtipo = $('#comboboxTipoDerivacion option:selected').val();


        if (codtipo == "-1") {
            $('#divDerivacionOtroEquipo').hide();
            $('#divDerivacionMismoEquipo').hide();
        } else if (codtipo == "1") {

            $.ajax({
                type: 'GET',
                data: { codequipo: codequipo },
                url: '/Usuario/ListarUsuarios/',//'@Url.Action("ListarSubTipos", "Ticket")',
                success: function (result) {
                    var a = '<option value="-1">Seleccione un Usuario</option>';
                    for (var i = 0; i < result.length; i++) {
                        a += '<option value="' + result[i].matricula + '">' + result[i].nombrecompleto + '</option>';
                    }
                    $('#combonuevousuario').html(a);
                }
            });

            $('#divDerivacionOtroEquipo').hide();
            $('#divDerivacionMismoEquipo').show();
        } else {


            $.ajax({
                type: 'GET',
                //data: { codequipo: codequipo },
                url: '/Equipo/ListarEquipos/',//'@Url.Action("ListarSubTipos", "Ticket")',
                success: function (result) {
                    var a = '<option value="-1">Seleccione un Equipo</option>';
                    for (var i = 0; i < result.length; i++) {
                        a += '<option value="' + result[i].codequipo + '">' + result[i].equipo1 + '</option>';
                    }
                    $('#combonuevoequipo').html(a);
                }
            });


            $('#divDerivacionOtroEquipo').show();
            $('#divDerivacionMismoEquipo').hide();
        }
    });


    $('#combonuevoequipo').on('change', function () {

        var codequipo = $('#combonuevoequipo option:selected').val();
        $.ajax({
            type: 'GET',
            data: { codequipo: codequipo },
            url: '/Aplicativo/ListarAplicativos/',//'@Url.Action("ListarAplicativos","Ticket")',
            success: function (result) {
                var a = '<option value="-1">Seleccione un Aplicativo o Cartera</option>';
                for (var i = 0; i < result.length; i++) {
                    a += '<option value="' + result[i].codaplicativo + '">' + result[i].aplicativo1 + '</option>';
                }
                $('#combonuevoaplicativo').html(a);
            }
        });
    });

    $('#btnGuardar').click(function () {

        var error = ValidarRegistrar();

        if (error) {
            //Declaracion de Objeto
            nuevoTicket = {};

            //Obtenemos los datos de la vista
            nuevoTicket.asunto = $('#txtasunto').val();
            nuevoTicket.descripcion = $('#txtdetalle').val();
            nuevoTicket.codequipo = $('#comboboxEquipo').val();
            nuevoTicket.codaplicativo = $('#comboboxAplicativo').val();
            nuevoTicket.codtipo = $('#comboboxTipo').val();
            nuevoTicket.matricula_modelo = $('#txtmatriculamodelo').val();


            $.ajax({
                url: '/Ticket/RegistrarTicket/',
                type: 'POST',
                dataType: 'json',
                data: nuevoTicket,
                success: function (response) {
                    if (response.success) {
                        $.notify(response.message, "success");
                        $('#txtasunto').val('');
                        $('#txtdetalle').val('');
                        $('#comboboxEquipo').attr('selected', false);
                        $('#comboboxAplicativo').attr('selected', false);
                        $('#comboboxTipo').attr('selected', false);
                        //window.location.href =  '@Url.Action("MostrarTicket", "Ticket")'
                    }
                    else {
                        $.notify(response.message, "error");
                    }
                }

            });

        }

    });


    $('#btnAtender').click(function () {

        var error = ValidarAtender();
        var codtipo = $('#codtipo').val();
        var codequipo = $('#codequipo').val();

        if (error) {
            Ticket = {};

            Ticket.idticket = $('#idticket').val();
            Ticket.comentariofinal = $('#txtrespuesta').val();

            if (codequipo == "1") {
                if (codtipo == "2") {
                    Ticket.causa = $('#txtcausaraiz').val();
                    Ticket.accion_inmediata = $('#txtaccioninmediata').val();
                    Ticket.codresponsable = $('#comboboxResponsable').val();
                }
            } else if (codequipo == "2") {
                Ticket.causa = $('#txtcausaraiz').val();
                Ticket.accion_inmediata = $('#txtaccioninmediata').val();
                Ticket.cantidad_clientes = $('#txtcantidadclientes').val();
                Ticket.cantidad_cuentas = $('#txtcantidadcuentas').val();
                Ticket.deuda_vencida = $('#txtdeudavencida').val();
                Ticket.deuda_total = $('#txtdeudatotal').val();
            }

            

            $.ajax({
                url: '/Ticket/AtenderTicket/',
                type: 'POST',
                dataType: 'json',
                data: Ticket,


                success: function (response) {

                    window.location.href = '/Ticket/MostrarTicketAsignados?msg=atendido';

                },
                error: function (xhr, status, error) {
                    alert(error);
                }

            });
        }
    });


    $('#btnDerivar').click(function () {

        var tipoderivacion = $('#comboboxTipoDerivacion').val();

        if (tipoderivacion == -1) {
            nowuiDashboard.showNotification('top', 'right', 'Debe elegir un tipo de derivación');
        } else if (tipoderivacion == 1) {
            var error = ValidarDerivacionMismoEquipo();

            if (error) {
                Ticket = {};

                Ticket.idticket = $('#idticket').val();
                Ticket.matricula_responsable = $('#matricula').val();
                Ticket.matricula_nueva = $('#combonuevousuario').val();
                Ticket.motivo = $('#txtrazonderivacion1').val();
                Ticket.tipoderivacion = tipoderivacion;

                $.ajax({
                    url: '/Ticket/DerivarTicket/',
                    type: 'POST',
                    dataType: 'json',
                    data: Ticket,

                    success: function (response) {

                        window.location.href = '/Ticket/MostrarTicketAsignados?msg=derivado';

                    },
                    error: function (xhr, status, error) {
                        alert(error);
                    }
                });
            }

        } else {
            var error = ValidarDerivacionOtroEquipo();

            if (error) {
                Ticket = {};

                Ticket.idticket = $('#idticket').val();
                Ticket.matricula_responsable = $('#matricula').val();
                Ticket.codequipo = $('#combonuevoequipo').val();
                Ticket.codaplicativo = $('#combonuevoaplicativo').val();
                Ticket.motivo = $('#txtrazonderivacion2').val();
                Ticket.tipoderivacion = tipoderivacion;

                $.ajax({
                    url: '/Ticket/DerivarTicket/',
                    type: 'POST',
                    dataType: 'json',
                    data: Ticket,

                    success: function (response) {

                        window.location.href = '/Ticket/MostrarTicketAsignados?msg=derivado';

                    },
                    error: function (xhr, status, error) {
                        alert(error);
                    }
                });
            }
        }

    });


    function ValidarRegistrar() {

        var error = true;
        if ($('#comboboxEquipo').val() == -1) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe elegir un Equipo');
        }
        if ($('#comboboxAplicativo').val() == -1) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe elegir un Aplicativo o Cartera');
        }
        if ($('#comboboxTipo').val() == -1) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe elegir un Tipo de Cartera');
        }
        if ($('#txtasunto').val().length == 0) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe ingresar un asunto para el ticket');
        }
        if ($('#txtdetalle').val().length == 0) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe ingresar el detalle para el ticket');
        }

        //validando la matricula modelo
        if ($('#comboboxTipo').val() == 4 && $('#txtmatriculamodelo').val().length == 0) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe ingresar una matricula modelo');
        }

        return error;
    }

    function ValidarAtender() {
        var error = true;

        if ($('#txtrespuesta').val().length == 0) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe ingresar la respuesta detallada para el solicitante.');
        }

        return error;
    }

    function ValidarDerivacionOtroEquipo() {
        var error = true;

        if ($('#combonuevoequipo').val() == -1) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe ingresar al equipo al cual va a dirigir el ticket.');
        }

        if ($('#combonuevoaplicativo').val() == -1) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe ingresar el aplicativo o cartera.');
        }

        if ($('#txtrazonderivacion2').val().length == 0) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe ingresar la razon por la cual está derivando el ticket.');
        }

        return error;
    }

    function ValidarDerivacionMismoEquipo() {
        var error = true;

        if ($('#combonuevousuario').val() == -1) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe ingresar al nuevo especialista.');
        }

        if ($('#txtrazonderivacion1').val().length == 0) {
            error = false;
            nowuiDashboard.showNotification('top', 'right', 'Debe ingresar la razon por la cual está derivando el ticket.');
        }

        return error;
    }



});