function Vehicle(options, model) {
    let mode = options.mode;
    let $form = $("#" + options.form);
    let $modal = $("#" + options.modalId);

    let postbackId = "";
    let postbackClass = "";

    AutoNumeric.multiple(["input#Capacity"], develoverSettings.OptionAutoNumericPacking);
    /** Modal */

    $modal.on("shown.bs.modal", (e) => {
        postbackId = $(e.relatedTarget).data("postback-id");
        postbackClass = $(e.relatedTarget).data("postback-class");

        $form.find("#Id").val(uuidv4());
    })

    $modal.on("hidden.bs.modal", (e) => {
        postbackId = "";
        postbackClass = "";
        model.id = uuidv4();

        $("select", $form).selectpicker("val", "");
        $("input[type=\"text\"]", $form).val("");
        $("textarea", $form).val("");
        $("input#Status", $form).prop("checked", true);

        $form.find(".is-invalid").removeClass("is-invalid");
        $form.find(".invalid-feedback").html("");
    })

    this.hideModal = () => {
        $modal.modal("hide");
    }

    /** Modal */

    /** Master */

    function createModel() {
        model.registrationNo = $("input#RegistrationNo", $form).val();
        model.registrationDate = $("input#RegistrationDate", $form).val();
        model.manufactureDate = $("input#ManufactureDate", $form).val();
        model.purchaseDate = $("input#PurchaseDate", $form).val();
        model.engineNo = $("input#EngineNo", $form).val();
        model.chassisNo = $("input#ChassisNo", $form).val();
        model.brand = $("input#Brand", $form).val();
        model.modelNo = $("input#ModelNo", $form).val();
        model.color = $("input#Color", $form).val();
        model.capacity = $("input#Capacity", $form).val();
        model.defaultDriverId = $("select#DefaultDriverId", $form).val();
        model.defaultDepartmentId = $("select#DefaultDepartmentId", $form).val();
        model.note = $("textarea#Note", $form).val();
        model.status = $("input#Status", $form).prop("checked");
    }

    this.save = () => {

        if (!validateDataInput(["Name"], ["Name"], $form))
            return;

        createModel();

        $.ajax({
            url: "/catalog/savevehicle",
            type: "post",
            data: { mode, model }
        }).done((res) => {

            swal({
                title: "Success",
                text: "The Vehicle has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            }).then(() => {
                console.log()
                if (options.modalId && postbackId && postbackId !== "" && postbackClass && postbackClass !== "") {
                    if (model.status) {
                        $("select." + postbackClass).map((index, select) => {
                            $(select).append("<option value=\"" + res.id + "\">" + res.name + "</option>");
                            $(select).selectpicker("refresh");
                        });

                        $("#" + postbackId).selectpicker("val", res.id);
                        $("button[data-id=\"" + postbackId + "\"]").attr("title", res.name);
                        $(".filter-option-inner-inner", $("button[data-id=\"" + postbackId + "\"]")).text(res.name);
                    }

                    this.hideModal();
                }
                else {
                    window.location.href = "/catalog/detailvehicle?id=" + res.id;
                }
            })
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $form);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
        });
    }

    this.delete = () => {
        swal({
            title: "Delete Vehicle?",
            text: "Do you want to delete the vehicle now? This cannot be undone.",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((res) => {
            if (res) {
                $.ajax({
                    url: "/catalog/deletevehicle",
                    type: "post",
                    data: { id: model.id }
                }).done((res) => {
                    swal({
                        title: "Success",
                        text: "The vehicle has been deleted!",
                        icon: "success",
                        timer: develoverSettings.swal.timer,
                        closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                        closeOnEsc: develoverSettings.swal.closeOnEsc
                    }).then(() => {
                        window.location.href = "/catalog/vehicles";
                    })
                }).fail((err) => {
                    swal("[" + err.status + "] " + err.responseText, {
                        icon: "error",
                    });
                });
            }
        });
    }


    /** Master */


    this.initialize = () => {
        if (mode === CatalogMode.VIEW) {
            $form.find("input:not(.datepicker), textarea").prop("readonly", true);
            $form.find("input.datepicker, select, button, a, input[type=\"checkbox\"]").prop("disabled", true);
            $form.find("textarea.summernote").summernote("disable");
        }
    }
}