function Repair(options, model) {
    let mode = options.mode;
    let $form = $("#" + options.form);
    let $table = $("#" + options.tableId);
    let $modal = $("#" + options.modalId);

    /** Details */

    [Quantity] = AutoNumeric.multiple(["#" + options.modalId + " input#Quantity"], develoverSettings.OptionAutoNumericQuantity);
    [Price] = AutoNumeric.multiple(["#" + options.modalId + " input#Price"], develoverSettings.OptionAutoNumericAmount);
    [Amount] = AutoNumeric.multiple(["#" + options.modalId + " input#Amount"], develoverSettings.OptionAutoNumericAmountVND);

    window.operateEvents = {
        "click .edit": function (e, value, row, index) {
            $modal.data("rowData", row);
            $modal.data("rowIndex", index);
            $modal.modal("show");
        },
        "click .delete": function (e, value, row, index) {
            swal({
                title: "Delete row?",
                text: "Do you want to delete this row now? This cannot be undone.",
                icon: "warning",
                buttons: true,
                dangerMode: true,
            }).then((res) => {
                if (res) {
                    $table.bootstrapTable("remove", {
                        field: "id",
                        values: [row.id]
                    });
                }
            });
        }
    }

    $modal.on("show.bs.modal", (e) => {
        let rowData = $modal.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $modal.find("h5.modal-title").text("Add item");
            $modal.find("button.save").text("Add");
            $modal.find("button.save-close").removeClass("d-none");
        }
        else {
            $modal.find("h5.modal-title").text("Edit item");
            $modal.find("button.save").text("Save");
        }
    })

    $modal.on("shown.bs.modal", (e) => {
        let rowData = $modal.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $modal.find("input:not(.datepicker), textarea").val("");
            $modal.find("input.datepicker").datepicker("update", "");
            $modal.find("select.selectpicker").selectpicker("val", "");

            $modal.find("input#Id").val(uuidv4());
        }
        else {
            $modal.find("input#Id").val(rowData.id);
            $modal.find("select#VehicleCostId").selectpicker("val", rowData.vehicleCostId);
            Quantity.set(rowData.quantity);
            Price.set(rowData.price);
            Amount.set(rowData.amount);
            $modal.find("textarea#Note").val(rowData.note);

        }
    })

    $modal.on("hidden.bs.modal", (e) => {
        $modal.find("button.save-close").addClass("d-none");

        $modal.find("input:not(.datepicker), textarea").val("");
        $modal.find("input.datepicker").datepicker("update", "");
        $modal.find("select.selectpicker").selectpicker("val", "");

        $modal.find(".is-invalid").removeClass("is-invalid");
        $modal.find(".invalid-feedback").html("");

        $modal.removeData("rowIndex");
        $modal.removeData("rowData");
    })

    $("select#VehicleCostId", $modal).on("change", (e) => {
        //let selectedValue = $(this).val();
    })

    //calcAmount
    $("input#Price ,input#Quantity", $modal).on("change", (e) => {
        Amount.set(0);
        calcAmount();
    })
    $("input#Amount", $modal).on("change", (e) => {
        calcAmount();
    })
    function calcAmount() {
        if (Amount.getNumber() === 0) {
            Amount.set(Quantity.getNumber() * Price.getNumber());
        }
    }

    //END calcAmount

    this.addItem = (closeAfterAdd) => {
        if (!validateDataInput(["VehicleCostId"], ["Cost type"], $modal))
            return;

        let index = $modal.data("rowIndex");

        if (index === undefined) {

            index = $table.bootstrapTable("getData").length;

            $table.bootstrapTable("insertRow", {
                index: index,
                row: {
                    id: $modal.find("input#Id").val(),
                    vehicleCostId: $modal.find("select#VehicleId").val(),
                    vehicleCost: $modal.find("select#VehicleId option:selected").text(),
                    quantity: Quantity.getNumber(),
                    price: Price.getNumber(),
                    amount: Amount.getNumber(),
                    note: $modal.find("textarea#Note").val(),
                }
            });
        }
        else {
            $table.bootstrapTable("updateRow", {
                index: index,
                row: {
                    id: $modal.find("input#Id").val(),
                    vehicleCostId: $modal.find("select#VehicleId").val(),
                    vehicleCost: $modal.find("select#VehicleId option:selected").text(),
                    quantity: Quantity.getNumber(),
                    price: Price.getNumber(),
                    amount: Amount.getNumber(),
                    note: $modal.find("textarea#Note").val(),
                }
            });
            $modal.removeData("rowIndex");
            closeAfterAdd = true;
        }

        if (closeAfterAdd) {
            $modal.modal("hide");
        }
        else {
            $("input#Id", $modal).val(uuidv4());
        }
    }

    this.hideModal = () => {
        $modal.modal("hide");
    }

    /** Details */


    /** Master */

    [EstimatedRepairCost] = AutoNumeric.multiple(["input#EstimatedRepairCost"], develoverSettings.OptionAutoNumericAmountVND);

    function createModel() {
        let items = [];

        $table.bootstrapTable("getData").map((row, index) => {
            let item = {
                id: row.id,
                vehicleRepairId: model.id,
                sequenceNo: index + 1,
                vehicleCostId: row.vehicleCostId,
                quantity: row.quantity,
                price: row.price,
                amount: row.amount,
                note: row.note
            }

            items.push(item);
        });

        model.issueDate = $("input#IssueDate", $form).val();
        model.no = $("input#No", $form).val();
        model.vehicleId = $("select#VehicleId", $form).val();
        model.driverId = $("select#DriverId", $form).val();
        model.repairShop = $("input#RepairShop", $form).val();
        model.estimatedCompletionDate = $("input#EstimatedCompletionDate", $form).val();
        model.estimatedRepairCost = EstimatedRepairCost.getNumber();
        model.complete = $("input#Complete", $form).prop("checked");
        model.note = $("textarea#Note", $form).val();
        model.items = items;
    }

    this.save = () => {
        if (!validateDataInput(["No", "VehicleId", "DriverId"], ["No", "Vehicle", "Driver"], $form))
            return;

        createModel();

        $.ajax({
            url: "/vehicle/repair/save",
            type: "post",
            data: { mode, model },
        }).done((res) => {
            swal({
                title: "Success",
                text: "The vehicle repair has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            }).then(() => {
                window.location.href = "/vehicle/repair/detail?id=" + res.id;
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
            title: "Delete Vehicle Repair?",
            text: "Do you want to delete the vehicle repair now? This cannot be undone.",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((res) => {
            if (res) {
                $.ajax({
                    url: "/vehicle/repair/delete",
                    type: "post",
                    data: { id: model.id }
                }).done((res) => {
                    swal({
                        title: "Success",
                        text: "The vehicle repair has been deleted!",
                        icon: "success",
                        timer: develoverSettings.swal.timer,
                        closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                        closeOnEsc: develoverSettings.swal.closeOnEsc
                    }).then(() => {
                        window.location.href = "/vehicle/repair";
                    })
                }).fail((err) => {
                    swal("[" + err.status + "] " + err.responseText, {
                        icon: "error",
                    });
                });
            }
        });
    }

    this.cancel = () => {
        swal({
            title: "Discard Document Changes?",
            text: "Do you want to discard document changes now? This cannot be undone.",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((res) => {
            if (res) {
                //$.ajax({
                //    url: "/documents/inbox/cancel",
                //    type: "post",
                //    data: { mode, model }
                //}).done((res) => {
                //    if (mode === DocumentMode.EDIT) {
                //        window.location.href = "/documents/inbox/detail?id=" + model.id;
                //    }
                //    else {
                history.back();
                //    }
                //}).fail((err) => {
                //    swal("[" + err.status + "] " + err.responseText, {
                //        icon: "error",
                //    });
                //});
            }
        });
    }


    /** Master */
    this.Complete = () => {
        var id = $("select#VehicleId", $form).val();
        $.ajax({
            url: "/vehicle/home/compeleteRepairing",
            type: "post",
            data: { idVehicle: id },
            async: false
        }).done(() => {
            window.location.reload();
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
    };


    this.initialize = () => {
        let columns = model.detailColumns;

        if (mode === VoucherMode.VIEW) {
            columns.pop(); // remove operate column
        }

        initBootstrapTable(options.tableId, columns, [], model.detailDataUrl);

        if (mode === VoucherMode.VIEW) {
            $form.find("input:not(.datepicker), textarea").prop("readonly", true);
            $form.find("input.datepicker, select, button, a,input[type=\"checkbox\"]").prop("disabled", true);

            $form.find(".add-item").remove();
        }
        $form.find("input#No").prop("disabled", true);

        createNo();
    }

    $('input#No', $form).on('change', function () {
        createNo();
    })
    function createNo() {
        let no = $('input#No');

        if (no.val() === "") {
            let id = $('input#Id').val()
            $.ajax({
                url: '/vehicle/repair/getno',
                type: 'get',
                data: { id: id }
            }).done((res) => {
                no.val(res.no);
            }).fail((err) => {
                console.log(err)
            });
        }
    }
}