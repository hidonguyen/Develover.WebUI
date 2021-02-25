const VoucherMode = Object.freeze({ "NEW": 1, "EDIT": 2, "VIEW": 3 });

function validateDataInput(fieldId, fieldDesc, context) {
    let isValid = true;

    fieldId.map((field, index) => {
        let input = $('#' + field, context);

        if (input.val() === null || input.val() === '') {
            if (input.hasClass('selectpicker')) {
                input.closest('div.bootstrap-select').addClass('is-invalid');
            }
            else {
                input.addClass('is-invalid');
            }
            $('.invalid-feedback', input.closest('div.field-group')).html(fieldDesc[index] + ' is required');

            isValid = false;
        }
        else {
            if (input.hasClass('selectpicker')) {
                input.closest('div.bootstrap-select').removeClass('is-invalid');
            }
            else {
                input.removeClass('is-invalid');
            }
            $('.invalid-feedback', input.closest('div.field-group')).html('');
        }
    });

    return isValid;
}

function VehicleInfo(options, model) {
    let $form = $("#" + options.form);
    let $modalDriving = $("#" + options.modalDriving);
    let $modalScheduled = $("#" + options.modalScheduled);
    let $modalRepairing = $("#" + options.modalRepairing);

    let $vehicleInfo_Id = $('#VehicleInfo_Id');
    let $vehicleInfo_DefaultDepartmentId = $('#VehicleInfo_DefaultDepartmentId');
    let $vehicleInfo_DefaultDriverId = $('#VehicleInfo_DefaultDriverId');
    let $vehicleInfo_CurrentVehicleRepairId = $('#VehicleInfo_CurrentVehicleRepairId');
    let $vehicleInfo_CurrentVehicleScheduleId = $('#VehicleInfo_CurrentVehicleScheduleId');

    let btnfuncdriving = $('.btn-func-car a[data-btn-func-car=\'btn-func-driving\']');
    let btnfuncscheduled = $('.btn-func-car a[data-btn-func-car=\'btn-func-scheduled\']');
    let btnfuncrepairing = $('.btn-func-car a[data-btn-func-car=\'btn-func-repairing\']');

    var that = this;
    $modalDriving.on("hidden.bs.modal", (e) => {
        $modalDriving.find("input:not(.datepicker), textarea").val("");
        $modalDriving.find("input.datepicker").datepicker("update", "");
        $modalDriving.find("select.selectpicker").selectpicker("val", "");
        $modalDriving.find(".is-invalid").removeClass("is-invalid");
        $modalDriving.find(".invalid-feedback").html("");

    });

    $modalScheduled.on("hidden.bs.modal", (e) => {
        $modalScheduled.find("input:not(.datepicker), textarea").val("");
        $modalScheduled.find("input.datepicker").datepicker("update", "");
        $modalScheduled.find("select.selectpicker").selectpicker("val", "");
        $modalScheduled.find(".is-invalid").removeClass("is-invalid");
        $modalScheduled.find(".invalid-feedback").html("");


    });

    $modalRepairing.on("hidden.bs.modal", (e) => {
        $modalScheduled.find("input:not(.datepicker), textarea").val("");
        $modalScheduled.find("input.datepicker").datepicker("update", "");
        $modalScheduled.find("select.selectpicker").selectpicker("val", "");
        $modalScheduled.find(".is-invalid").removeClass("is-invalid");
        $modalScheduled.find(".invalid-feedback").html("");
    });

    this.LoadInfo = (id) => {
        $.ajax({
            url: "/vehicle/home/GetInfoVehicle",
            type: "post",
            data: { id },
        }).done((res) => {
            loadInfo(res.vehicleInfo);
            loadStatusCar(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing);
            loadStatusCarButton(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing, res.vehicleInfo.defaultDepartmentId, res.vehicleInfo.defaultDriverId, res.vehicleInfo.currentVehicleRepairId, res.vehicleInfo.currentVehicleScheduleId);

            scheduleRefresh();
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
            return false;
        });
        return true;
    };

    this.LoadStatus = () => {
        $.ajax({
            url: "/vehicle/home/GetStatusAll",
            type: "get",
        }).done((res) => {
            loadStatus(res.list);
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
            return false;
        });
        return true;
    };

    this.UpdateStatusDriving = () => {
        if (btnfuncrepairing.attr("data-btn-func-car-value") === 'true') return;

        if (!validateDataInput(["DriverId", "DepartmentId"], ["Driver", "Department"], $modalDriving))
            return;

        let id = $vehicleInfo_Id.val();

        let idScheduled = $modalDriving.find('#ScheduledId').val();
        let idDriver = $modalDriving.find('#DriverId').val();
        let idDepartment = $modalDriving.find('#DepartmentId').val();
        let note = $modalDriving.find('#Note').val();

        if (updateStatusDriving(id, idScheduled, idDriver, idDepartment, note))
            this.hideModalDriving();
    };

    $modalDriving.find('#ScheduledId').on('change', (e) => {
        let id = $modalDriving.find('#ScheduledId').val();
        $.ajax({
            url: "/vehicle/home/getScheduledInfo",
            type: "post",
            data: { id },
        }).done((res) => {
            $modalDriving.find('#DriverId').selectpicker("val", res.driverId);
            $modalDriving.find('#DepartmentId').selectpicker("val", res.departmentId);
        });
    });

    function updateStatusDriving(id, idScheduled, idDriver, idDepartment, note) {
        let status = true;
        $.ajax({
            url: "/vehicle/home/UpdateStatusDriving",
            type: "post",
            data: { idVehicle: id, idScheduled, idDriver: idDriver, idDepartment: idDepartment, note: note },
        }).done((res) => {
            loadInfo(res.vehicleInfo);
            loadStatusCar(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing);
            loadStatusCarButton(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing, res.vehicleInfo.defaultDepartmentId, res.vehicleInfo.defaultDriverId, res.vehicleInfo.currentVehicleRepairId, res.vehicleInfo.currentVehicleScheduleId);
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
            status = false;
        });
        return status;
    };

    this.UpdateStatusScheduled = () => {

        if (btnfuncrepairing.attr("data-btn-func-car-value") === 'true') return;

        if (!validateDataInput(["DriverId", "PetitionerId", "DepartmentId", "VehicleUsePurposeId"], ["Driver", "Petitioner", "Department", "VehicleUse Purpose"], $modalScheduled))
            return;

        let id = $vehicleInfo_Id.val();

        let departureDate = $modalScheduled.find('#DepartureDate').val();
        let returnDate = $modalScheduled.find('#ReturnDate').val();
        let idDriver = $modalScheduled.find('#DriverId').val();
        let idPetitioner = $modalScheduled.find('#PetitionerId').val();
        let idDepartment = $modalScheduled.find('#DepartmentId').val();
        let idVehicleUsePurpose = $modalScheduled.find('#VehicleUsePurposeId').val();
        let destination = $modalScheduled.find('#Destination').val();
        if (updateStatusScheduled(id, departureDate, returnDate, idDriver, idPetitioner, idDepartment, destination, idVehicleUsePurpose)) {
            this.hideModalScheduled();
        }
    };

    function updateStatusScheduled(id, departureDate, returnDate, idDriver, idPetitioner, idDepartment, destination, idVehicleUsePurpose) {
        let status = true;
        $.ajax({
            url: "/vehicle/home/UpdateStatusScheduled",
            type: "post",
            data: { idVehicle: id, departureDate, returnDate, idDriver, idPetitioner, idDepartment, destination, idVehicleUsePurpose },
            async: false
        }).done((res) => {
            loadInfo(res.vehicleInfo);
            loadStatusCar(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing);
            loadStatusCarButton(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing, res.vehicleInfo.defaultDepartmentId, res.vehicleInfo.defaultDriverId, res.vehicleInfo.currentVehicleRepairId, res.vehicleInfo.currentVehicleScheduleId);
            scheduleRefresh();
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
            status = false;
        });
        return status;
    };

    function compeleteScheduled(id) {
        let status = true;
        $.ajax({
            url: "/vehicle/home/compeleteScheduled",
            type: "post",
            data: { idVehicle: id },
            async: false
        }).done((res) => {
            loadInfo(res.vehicleInfo);
            loadStatusCar(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing);
            loadStatusCarButton(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing, res.vehicleInfo.defaultDepartmentId, res.vehicleInfo.defaultDriverId, res.vehicleInfo.currentVehicleRepairId, res.vehicleInfo.currentVehicleScheduleId);
            scheduleRefresh();
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
            status = false;
        });
        return status;
    };

    this.UpdateStatusRepairing = () => {
        if (btnfuncdriving.attr("data-btn-func-car-value") === 'true') return;
        if (btnfuncrepairing.attr("data-btn-func-car-value") === 'true') return;
        if (!validateDataInput(["DriverId", "PetitionerId", "DepartmentId"], ["Driver", "Petitioner", "Department"], $modalRepairing))
            return;

        let id = $vehicleInfo_Id.val();

        let idDriver = $modalRepairing.find('#DriverId').val();
        let repairShop = $modalRepairing.find('#RepairShop').val();
        let estimatedCompletionDate = $modalRepairing.find('#EstimatedCompletionDate').val();
        let estimatedRepairCost = $modalRepairing.find('#EstimatedRepairCost').val();

        if (updateStatusRepairing(id, idDriver, repairShop, estimatedCompletionDate, estimatedRepairCost))
            this.hideModalRepairing();
    };

    function updateStatusRepairing(id, idDriver, repairShop, estimatedCompletionDate, estimatedRepairCost) {
        let status = true;
        $.ajax({
            url: "/vehicle/home/UpdateStatusRepairing",
            type: "post",
            data: { idVehicle: id, idDriver, repairShop, estimatedCompletionDate, estimatedRepairCost },
            async: false
        }).done((res) => {
            loadInfo(res.vehicleInfo);
            loadStatusCar(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing);
            loadStatusCarButton(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing, res.vehicleInfo.defaultDepartmentId, res.vehicleInfo.defaultDriverId, res.vehicleInfo.currentVehicleRepairId, res.vehicleInfo.currentVehicleScheduleId);
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
            status = false;
        });
        return status;
    };

    function compeleteRepairing(id) {
        let status = true;

        $.ajax({
            url: "/vehicle/home/compeleteRepairing",
            type: "post",
            data: { idVehicle: id },
            async: false
        }).done((res) => {
            loadInfo(res.vehicleInfo);
            loadStatusCar(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing);
            loadStatusCarButton(res.vehicleInfo.id, res.vehicleInfo.isDriving, res.vehicleInfo.isScheduled, res.vehicleInfo.isRepairing, res.vehicleInfo.defaultDepartmentId, res.vehicleInfo.defaultDriverId, res.vehicleInfo.currentVehicleRepairId, res.vehicleInfo.currentVehicleScheduleId);
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
            status = false;
        });
        return status;
    };

    function loadStatus(list) {
        list.map((value, index) => {
            loadStatusCar(value.id, value.isDriving, value.isScheduled, value.isRepairing);
        });

        if ($('.vehicles.card-body .card.vehicle.active-vehicle').length) {
            let id = $vehicleInfo_Id.val();
            if (id != '00000000-0000-0000-0000-000000000000')
                that.LoadInfo(id);
        }
    }

    function loadStatusCar(id, isDriving, isScheduled, isRepairing) {

        let statusClass = "fa-car-garage text-secondary";

        if (isScheduled) {
            statusClass = "fa-car-garage text-primary";
        }

        if (isDriving) {
            statusClass = "fa-car-side text-success";
        }
        else if (isRepairing) {
            statusClass = "fa-car-mechanic text-danger";
        }

        $('a#vehicle_' + id + ' i').removeClass().addClass('fad ' + statusClass);
        $('a#vehicle_' + id + ' i span').removeClass('d-none').addClass(isScheduled ? '' : 'd-none');
    }

    function loadStatusCarButton(id, isDriving, isScheduled, isRepairing, defaultDepartmentId, defaultDriverId, currentVehicleRepairId, currentVehicleScheduleId) {
        let classbtnbase = 'ml-2 btn btn-sm';
        let classbtnbasesecondary = 'btn-outline-secondary';

        btnfuncdriving.removeClass().addClass(classbtnbase).addClass(classbtnbasesecondary);
        btnfuncscheduled.removeClass().addClass(classbtnbase).addClass(classbtnbasesecondary);
        btnfuncrepairing.removeClass().addClass(classbtnbase).addClass(classbtnbasesecondary);

        btnfuncdriving.removeClass('d-none');
        btnfuncscheduled.removeClass('d-none');
        btnfuncrepairing.removeClass('d-none');

        btnfuncdriving.text('Drive');
        btnfuncscheduled.text('Schedule');
        btnfuncrepairing.text('Repair');

        $vehicleInfo_Id.val(id);
        $vehicleInfo_DefaultDepartmentId.val(defaultDepartmentId);
        $vehicleInfo_DefaultDriverId.val(defaultDriverId);
        $vehicleInfo_CurrentVehicleRepairId.val(currentVehicleRepairId);
        $vehicleInfo_CurrentVehicleScheduleId.val(currentVehicleScheduleId);

        btnfuncdriving.attr("data-btn-func-car-value", false);
        btnfuncscheduled.attr("data-btn-func-car-value", false);
        btnfuncrepairing.attr("data-btn-func-car-value", false);

        if (isDriving) {
            btnfuncdriving.removeClass(classbtnbasesecondary).addClass(classbtnbase);
            let currentVehicleScheduledId = $vehicleInfo_CurrentVehicleScheduleId.val();
            if (currentVehicleScheduledId != '00000000-0000-0000-0000-000000000000' && currentVehicleScheduledId != '') {
                btnfuncdriving.addClass('btn-outline-danger');
            } else {
                btnfuncdriving.addClass('btn-outline-warning');
            }

            btnfuncdriving.text('Driving');
            btnfuncdriving.attr("data-btn-func-car-value", true);

            btnfuncrepairing.addClass('d-none');
        }

        if (isScheduled) {
            btnfuncscheduled.removeClass(classbtnbasesecondary).addClass(classbtnbase).addClass('btn-outline-primary');
            btnfuncscheduled.text('Scheduled');
            btnfuncscheduled.attr("data-btn-func-car-value", true);
        }

        if (isRepairing) {
            btnfuncdriving.addClass('d-none');
            btnfuncscheduled.addClass('d-none');

            btnfuncrepairing.removeClass(classbtnbasesecondary).addClass(classbtnbase).addClass('btn-outline-danger');
            btnfuncrepairing.text('Repairing');
            btnfuncrepairing.attr("data-btn-func-car-value", true);
        }

    }

    function loadInfo(model) {
        $form.find("span#Id").text(model.id);
        $form.find("span#DefaultDriverId").text(model.defaultDriverId);
        $form.find("span#DefaultDepartmentId").text(model.defaultDepartmentId);
        $form.find("span#CurrentVehicleRepairId").text(model.currentVehicleRepairId);
        $form.find("span#CurrentVehicleScheduleId").text(model.currentVehicleScheduleId);
        $form.find("span#RegistrationNo").text(model.registrationNo);
        $form.find("span#EngineNo").text(model.engineNo);
        $form.find("span#ChassisNo").text(model.chassisNo);
        $form.find("span#Brand").text(model.brand);
        $form.find("span#ModelNo").text(model.modelNo);
        $form.find("span#Color").text(model.color);
        $form.find("span#Capacity").text(model.capacity);
        $form.find("span#DefaultDriver").text(model.defaultDriver);
        $form.find("span#DefaultDepartment").text(model.defaultDepartment);
        $form.find("span#IsScheduled").text(model.isScheduledText);
        $form.find("span#IsRepairing").text(model.isRepairingText);
        $form.find("span#IsDriving").text(model.isDrivingText);
        $form.find("span#Note").text(model.note);
    };

    this.initialize = () => {
        $vehicleInfo_Id.val('00000000-0000-0000-0000-000000000000');
        $vehicleInfo_DefaultDepartmentId.val('00000000-0000-0000-0000-000000000000');
        $vehicleInfo_DefaultDriverId.val('00000000-0000-0000-0000-000000000000');
        $vehicleInfo_CurrentVehicleRepairId.val('00000000-0000-0000-0000-000000000000');
        $vehicleInfo_CurrentVehicleScheduleId.val('00000000-0000-0000-0000-000000000000');

        btnfuncdriving.attr("data-btn-func-car-value", false);
        btnfuncscheduled.attr("data-btn-func-car-value", false);
        btnfuncrepairing.attr("data-btn-func-car-value", false);
        loadInfo(model.vehicleInfo);
    };

    this.hideModalDriving = () => {
        $modalDriving.modal("hide");
    }

    this.ShowModalDriving = () => {

        let currentVehicleScheduledId = $vehicleInfo_CurrentVehicleScheduleId.val();
        if (btnfuncdriving.attr("data-btn-func-car-value") === 'true') {
            if (currentVehicleScheduledId != '00000000-0000-0000-0000-000000000000') {
                swal({
                    title: "The vehicle is runing for scheduled?",
                    text: "Vehicle is runing for scheduled, complete or transfer to scheduled voucher?",
                    icon: "warning",
                    buttons: {
                        cancel: "Cancel",
                        Complete: {
                            text: "Complete!",
                            value: "complete",
                        },
                        Goto: {
                            text: "Transfer to!",
                            value: "goto",
                        },
                    },
                }).then((res) => {
                    switch (res) {
                        case "goto":
                            if (currentVehicleScheduledId === "")
                                return;
                            window.open(
                                '/vehicle/schedule/detail?id=' + currentVehicleScheduledId,
                                '_blank' // <- This is what makes it open in a new window.
                            );
                            break;
                        case "complete":
                            let id = $vehicleInfo_Id.val();
                            if (id == '00000000-0000-0000-0000-000000000000')
                                return;
                            compeleteScheduled(id);
                            break;
                    }
                });
            }
        } else {
            $modalDriving.modal("show");
        }
    };

    function scheduleRefresh() {
        let id = $vehicleInfo_Id.val();
        if (id == '00000000-0000-0000-0000-000000000000')
            return;
        $.ajax({
            url: "/vehicle/home/GetListScheduled",
            type: 'get',
            async: false,
            data: { idVehicle: id }
        }).done((res) => {

            $modalDriving.find('select#ScheduledId option:not([disabled])').remove();
            $modalDriving.find('select#ScheduledId').selectpicker('refresh');

            res.list.map((value, index) => {
                $modalDriving.find('select#ScheduledId').append('<option value="' + value.value + '">' + value.text + '</option>');
            })
            $modalDriving.find('select#ScheduledId').selectpicker('refresh');
        }).fail((err) => {
            console.log(err)
        });
    }

    this.hideModalScheduled = () => {
        $modalScheduled.modal("hide");
    }

    this.ShowModalScheduled = () => {
        $modalScheduled.modal("show");
    };

    this.hideModalRepairing = () => {
        $modalRepairing.modal("hide");
    }

    this.ShowModalRepairing = () => {
        if (btnfuncrepairing.attr("data-btn-func-car-value") === 'true') {

            swal({
                title: "The vehicle is repairing?",
                text: "Vehicle is being repaired, complete or transfer to repairing voucher?",
                icon: "warning",
                buttons: {
                    cancel: "Cancel",
                    Complete: {
                        text: "Complete!",
                        value: "complete",
                    },
                    Goto: {
                        text: "Transfer to!",
                        value: "goto",
                    },
                },
            }).then((res) => {
                switch (res) {
                    case "goto":
                        let currentVehicleRepairId = $vehicleInfo_CurrentVehicleRepairId.val();
                        if (currentVehicleRepairId == '00000000-0000-0000-0000-000000000000')
                            return;
                        window.open(
                            '/vehicle/repair/detail?id=' + currentVehicleRepairId,
                            '_blank' // <- This is what makes it open in a new window.
                        );
                        break;
                    case "complete":
                        let id = $vehicleInfo_Id.val();
                        if (id == '00000000-0000-0000-0000-000000000000')
                            return;
                        compeleteRepairing(id);
                        break;
                }
            });
        }
        else
            $modalRepairing.modal("show");
    };

}
$('.vehicle a').on('click', (e) => {
    $('.vehicles .card.vehicle').removeClass("active-vehicle");
    $(e.currentTarget.closest('.vehicle')).addClass('active-vehicle');
    if (!$('body').hasClass('offcanvas-active')) {
        $('body').removeClass('right_tb_toggle')
    } else {
        $('body').addClass('right_tb_toggle')
    }
});

