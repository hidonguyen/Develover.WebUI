function Currency(options, model) {
    let mode = options.mode;
    let $form = $("#" + options.form);
    let $modal = $("#" + options.modalId);

    let postbackId = "";
    let postbackClass = "";

    let ExchangeRate = new AutoNumeric("#" + options.form + " input#ExchangeRate", develoverSettings.OptionAutoNumericAmount)

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
        model.name = $("input#Name", $form).val();
        model.exchangeRate = ExchangeRate.getNumber();
        model.note = $("textarea#Note", $form).val();
        model.status = $("input#Status", $form).prop("checked");
    }

    this.save = () => {
        if (!validateDataInput(["Name"], ["Name"], $form))
            return;

        createModel();

        $.ajax({
            url: "/identity/savecurrency",
            type: "post",
            data: { mode, model }
        }).done((res) => {
            swal({
                title: "Success",
                text: "The currency has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            }).then(() => {
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
                    window.location.href = "/identity/detailcurrency?id=" + res.id;
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
            title: "Delete Currency?",
            text: "Do you want to delete the currency now? This cannot be undone.",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((res) => {
            if (res) {
                $.ajax({
                    url: "/identity/deletecurrency",
                    type: "post",
                    data: { id: model.id }
                }).done((res) => {
                    swal({
                        title: "Success",
                        text: "The currency has been deleted!",
                        icon: "success",
                        timer: develoverSettings.swal.timer,
                        closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                        closeOnEsc: develoverSettings.swal.closeOnEsc
                    }).then(() => {
                        window.location.href = "/identity/currencies";
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