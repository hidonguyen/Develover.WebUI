function OpeningStock(options, model) {
    let mode = options.mode;
    let $form = $("#" + options.form);
    let $table = $("#" + options.tableId);
    let $modal = $("#" + options.modalId);

    /** Details */

    [Quantity] = AutoNumeric.multiple(["#" + options.modalId + " input#Quantity"], develoverSettings.OptionAutoNumericQuantity);
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
            $modal.find("select#StockItemId").selectpicker("val", rowData.stockItemId);
            $modal.find("input#StockItem_UnitOfMeasure_Name").val(rowData.unitOfMeasure);
            $modal.find("select#LocationId").selectpicker("val", rowData.locationId);
            Quantity.set(rowData.quantity);
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

    $("select#StockItemId", $modal).on("change", (e) => {
        $.get("/stationery/openingstock/getuomofitem", { id: $(e.target).val() }, (res) => {
            $modal.find("input#StockItem_UnitOfMeasure_Name").val(res.name);
        });
        //let selectedValue = $(this).val();
    })

    $("select#LocationId", $modal).on("change", (e) => {
        //let selectedValue = $(this).val();
    })

    //END calcAmount

    this.addItem = (closeAfterAdd) => {
        if (!validateDataInput(["StockItemId", "LocationId"], ["Item", "Location"], $modal))
            return;

        let index = $modal.data("rowIndex");

        if (index === undefined) {

            index = $table.bootstrapTable("getData").length;

            $table.bootstrapTable("insertRow", {
                index: index,
                row: {
                    id: $modal.find("input#Id").val(),
                    stockItemId: $modal.find("select#StockItemId").val(),
                    stockItem: $modal.find("select#StockItemId option:selected").text(),
                    unitOfMeasure: $modal.find("input#StockItem_UnitOfMeasure_Name").val(),
                    locationId: $modal.find("select#LocationId").val(),
                    location: $modal.find("select#LocationId option:selected").text(),
                    quantity: Quantity.getNumber(),
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
                    stockItemId: $modal.find("select#StockItemId").val(),
                    stockItem: $modal.find("select#StockItemId option:selected").text(),
                    unitOfMeasure: $modal.find("input#StockItem_UnitOfMeasure_Name").val(),
                    locationId: $modal.find("select#LocationId").val(),
                    location: $modal.find("select#LocationId option:selected").text(),
                    quantity: Quantity.getNumber(),
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

    function createModel() {
        let items = [];

        $table.bootstrapTable("getData").map((row, index) => {
            let item = {
                id: row.id,
                goodsReceiptNoteId: model.id,
                sequenceNo: index + 1,
                stockItemId: row.stockItemId,
                locationId: row.locationId,
                quantity: row.quantity,
                amount: row.amount,
                note: row.note
            }

            items.push(item);
        });

        model.dateUpdate = $("input#DateUpdate", $form).val();
        model.note = $("textarea#Note", $form).val();
        model.items = items;
    }

    this.save = () => {

        createModel();
        console.log("111")
        $.ajax({
            url: "/stationery/openingstock/save",
            type: "post",
            data: { mode, model },
        }).done((res) => {
            swal({
                title: "Success",
                text: "The opening stock has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            }).then(() => {
                window.location.href = "/stationery/openingstock/detail?id=" + res.id;
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
            title: "Delete Opening Stock?",
            text: "Do you want to delete the opening stock now? This cannot be undone.",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((res) => {
            if (res) {
                $.ajax({
                    url: "/stationery/openingstock/delete",
                    type: "post",
                    data: { id: model.id }
                }).done((res) => {
                    swal({
                        title: "Success",
                        text: "The opening stock has been deleted!",
                        icon: "success",
                        timer: develoverSettings.swal.timer,
                        closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                        closeOnEsc: develoverSettings.swal.closeOnEsc
                    }).then(() => {
                        window.location.href = "/stationery/openingstock";
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
            title: "Discard opening stock Changes?",
            text: "Do you want to discard opening stock changes now? This cannot be undone.",
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


    this.initialize = () => {
        let columns = model.detailColumns;

        if (mode === VoucherMode.VIEW) {
            columns.pop(); // remove operate column
        }

        initBootstrapTable(options.tableId, columns, [], model.detailDataUrl);

        if (mode === VoucherMode.VIEW) {
            $form.find("input:not(.datepicker), textarea").prop("readonly", true);
            $form.find("input.datepicker, select, button, a").prop("disabled", true);

            $form.find(".add-item").remove();
        }

    }

}