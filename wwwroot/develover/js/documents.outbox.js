function OutboxDocument(options, model) {
    let mode = options.mode;
    let $form = $("#" + options.form);
    let $table = $("#" + options.tableId);
    let $modal = $("#" + options.modalId);
    let $tableAppendix = $("#" + options.tableAppendixId);
    let $modalAppendix = $("#" + options.modalAppendixId);
    /** Details */

    window.attachmentEvents = {
        "click .view": function (e, value, row, index) {
            let attachmentId = row.attachmentId;

            if (!attachmentId) attachmentId = row.id;

            window.open("/attachment/view?id=" + attachmentId, "_blank");
        },
        "click .download": function (e, value, row, index) {
            let attachmentId = row.attachmentId;

            if (!attachmentId) attachmentId = row.id;

            window.open("/attachment/download?id=" + attachmentId, "_blank");
        }
    }

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
                    model.currentAttachmentIds = model.currentAttachmentIds.filter(item => item != row.id);
                }
            });
        }
    }

    $modal.on("show.bs.modal", (e) => {
        let rowData = $modal.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $modal.find("h5.modal-title").text("Add attachment");
            $modal.find("button.save").text("Add");
        }
        else {
            $modal.find("h5.modal-title").text("Edit attachment");
            $modal.find("button.save").text("Save");
        }
    })

    $modal.on("shown.bs.modal", (e) => {
        let rowData = $modal.data("rowData"); // Extract info from data-* attributes
        let ownerId = $model.data("ownerId");
        if (!ownerId)
            ownerId = model.id;

        if (rowData === undefined) {
            $modal.find("input#AttachmentId").val(uuidv4());
            $modal.find("input#AttachmentOwnerId").val(ownerId);
            $modal.find("input#AttachmentAttachFile").val("");
            $modal.find("textarea#AttachmentNote").val("");
        }
        else {
            $modal.find("input#AttachmentId").val(rowData.id);
            $modal.find("input#AttachmentOwnerId").val(rowData.ownerId);
            $modal.find("input#AttachmentAttachFile").val("");
            $modal.find("textarea#AttachmentNote").val(rowData.note);
        }
    })

    $modal.on("hidden.bs.modal", (e) => {
        $modal.find("input#AttachmentId").val("");
        $modal.find("input#AttachmentOwnerId").val("");
        $modal.find("input#AttachmentAttachFile").val("");
        $modal.find("textarea#AttachmentNote").val("");

        $modal.find(".is-invalid").removeClass("is-invalid");
        $modal.find(".invalid-feedback").html("");

        $modal.removeData("rowIndex");
        $modal.removeData("rowData");
        $modal.removeData("ownerId");
    })

    this.addAttachment = (closeAfterAdd) => {
        let index = $modal.data("rowIndex");

        let ownerId = $modal.data("ownerId");
        if (!ownerId)
            ownerId = model.id;

        var formData = new FormData();
        formData.append("ownerId", ownerId);
        let attachFileInput = $("input#AttachmentAttachFile", $modal)[0];
        let attachFiles = attachFileInput.files;
        for (var i = 0; i != attachFiles.length; i++) {
            formData.append("files", attachFiles[i]);
        }
        formData.append("note", $("textarea#AttachmentNote", $modal).val());

        $.ajax({
            url: "/attachment/new",
            type: "post",
            processData: false,
            contentType: false,
            data: formData
        }).done((res) => {
            res.attachments.map((value, index) => {
                if (ownerId === model.id) { // Gọi từ form chính
                    if (index) { // Sửa
                        $table.bootstrapTable('updateRow', {
                            index: index,
                            row: {
                                id: value.id,
                                sequenceNo: value.sequenceNo,
                                ownerId: value.wwnerId,
                                name: value.name,
                                extension: value.extension,
                                fileContent: value.fileContent,
                                note: value.note
                            }
                        })
                    }
                    else { // Mới
                        index = $table.bootstrapTable('getData').length;
                        $table.bootstrapTable('insertRow', {
                            index: index,
                            row: {
                                id: value.id,
                                sequenceNo: value.sequenceNo,
                                ownerId: value.wwnerId,
                                name: value.name,
                                extension: value.extension,
                                fileContent: value.fileContent,
                                note: value.note
                            }
                        })
                    }
                }
                else { // Gọi từ appendix => update lại attachmentId
                    let originAppendix = $tableAppendix.bootstrapTable('getRowByUniqueId', ownerId);
                    $tableAppendix.bootstrapTable('updateByUniqueId', {
                        id: ownerId,
                        row: {
                            id: originAppendix.id,
                            ownerId: originAppendix.ownerId,
                            issueDate: originAppendix.issueDate,
                            appendixNo: originAppendix.appendixNo,
                            title: originAppendix.title,
                            description: originAppendix.description,
                            attachmentId: value.id
                        }
                    })
                }

                model.allAttachmentIds.push(value.id);
                model.currentAttachmentIds.push(value.id);
            });

            swal({
                title: "Success",
                text: "The file has been successfully attached to the document!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            });
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $modal);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
        });

        if (index === undefined) {
            if (closeAfterAdd) {
                this.hideModal();
            }
            else {
                $("input#AttachmentId", $modal).val(uuidv4());
            }
        }
        else {
            model.attachments = model.attachments.filter(item => item.id !== $("input#AttachmentId", $modal).val());

            $modal.removeData("rowIndex");
            this.hideModal();
        }
    }

    this.hideModal = () => {
        $modal.modal("hide");
    }

    window.attachmentAppendixEvents = {
        "click .upload": function (e, value, row, index) {
            $modal.data("ownerId", row.id);

            $modal.data("rowData", row);
            $modal.data("rowIndex", index);

            $modal.modal("show");
        },
        "click .download": function (e, value, row, index) {
            window.open("/attachment/download?id=" + row.attachmentId, "_blank");
        },
        "click .delete": function (e, value, row, index) {
            swal({
                title: "Delete attachment?",
                text: "Do you want to delete this attachment now? This cannot be undone.",
                icon: "warning",
                buttons: true,
                dangerMode: true,
            }).then((res) => {
                if (res) {
                    let originAppendix = $tableAppendix.bootstrapTable('getRowByUniqueId', row.id);
                    $tableAppendix.bootstrapTable('updateByUniqueId', {
                        id: row.id,
                        row: {
                            id: originAppendix.id,
                            ownerId: originAppendix.ownerId,
                            issueDate: originAppendix.issueDate,
                            appendixNo: originAppendix.appendixNo,
                            title: originAppendix.title,
                            description: originAppendix.description,
                            attachmentId: "00000000-0000-0000-0000-000000000000"
                        }
                    });

                    model.currentAttachmentIds = model.currentAttachmentIds.filter(item => item != row.attachmentId);
                }
            });
        }
    }

    window.operateAppendixEvents = {
        "click .edit": function (e, value, row, index) {
            $modalAppendix.data("rowData", row);
            $modalAppendix.data("rowIndex", index);
            $modalAppendix.modal("show");
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
                    $tableAppendix.bootstrapTable("remove", {
                        field: "id",
                        values: [row.id]
                    });
                    model.currentAttachmentIds = model.currentAttachmentIds.filter(item => item != row.attachmentId);
                }
            });
        }
    }

    $modalAppendix.on("show.bs.modal", (e) => {
        let rowData = $modalAppendix.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $modalAppendix.find("h5.modal-title").text("Add appendix");
            $modalAppendix.find("button.save").text("Add");
        }
        else {
            $modalAppendix.find("h5.modal-title").text("Edit appendix");
            $modalAppendix.find("button.save").text("Save");
        }
    })

    $modalAppendix.on("shown.bs.modal", (e) => {
        let rowData = $modalAppendix.data("rowData"); // Extract info from data-* attributes

        if (rowData === undefined) {
            $modalAppendix.find("input#Id").val(uuidv4());
            $modalAppendix.find("input#AttachmentId").val("00000000-0000-0000-0000-000000000000");

            $modalAppendix.find("input#IssueDate").datepicker("update", "");
            $modalAppendix.find("input#AppendixNo").val("");
            $modalAppendix.find("input#Title").val("");
            $modalAppendix.find("textarea#Description").val("");
        }
        else {
            $modalAppendix.find("input#Id").val(rowData.id);
            $modalAppendix.find("input#AttachmentId").val(rowData.attachmentId);

            $modalAppendix.find("input#IssueDate").datepicker("update", rowData.issueDate);
            $modalAppendix.find("input#AppendixNo").val(rowData.appendixNo);
            $modalAppendix.find("input#Title").val(rowData.title);
            $modalAppendix.find("textarea#Description").val(rowData.description);
        }
    })

    $modalAppendix.on("hidden.bs.modal", (e) => {
        $modalAppendix.find("input#Id").val("");
        $modalAppendix.find("input#AttachmentId").val("00000000-0000-0000-0000-000000000000");

        $modalAppendix.find("input#IssueDate").datepicker("update", "");
        $modalAppendix.find("input#AppendixNo").val("");
        $modalAppendix.find("input#Title").val("");
        $modalAppendix.find("textarea#Description").val("");

        $modalAppendix.find(".is-invalid").removeClass("is-invalid");
        $modalAppendix.find(".invalid-feedback").html("");

        $modalAppendix.removeData("rowIndex");
        $modalAppendix.removeData("rowData");
    })

    this.addAppendix = (closeAfterAdd) => {
        let index = $modalAppendix.data("rowIndex");
        let boostrapTableMethod = "updateRow";
        if (index === undefined) {
            index = $tableAppendix.bootstrapTable("getData").length - 1; //Trừ đi để đồng nhất với rowIndex => sequenceNo sẽ + 1
            boostrapTableMethod = "insertRow";
        }

        let appendix = {
            id: $modalAppendix.find("input#Id").val(),
            ownerId: model.id,
            issueDate: $modalAppendix.find("input#IssueDate").val(),
            appendixNo: $modalAppendix.find("input#AppendixNo").val(),
            title: $modalAppendix.find("input#Title").val(),
            description: $modalAppendix.find("textarea#Description").val(),
            attachmentId: $modalAppendix.find("input#AttachmentId").val(),
        }

        $.ajax({
            url: "/documents/outbox/validateappendixmodel",
            type: "post",
            data: { model: appendix }
        }).done((res) => {
            $tableAppendix.bootstrapTable(boostrapTableMethod, {
                index: index,
                row: res
            });
            $modalAppendix.modal("hide");
        }).fail((err) => {
            if (err.status === 400) {
                showHideValidateResult(JSON.parse(err.responseText), $modalAppendix);
            }
            else {
                swal("[" + err.status + "] " + err.responseText, {
                    icon: "error",
                });
            }
        });
    }

    this.hideAppendixModal = () => {
        $modalAppendix.modal("hide");
    }

    /** Details */


    /** Master */

    [TotalPages, TotalCopies] = AutoNumeric.multiple(["input#TotalPages", "input#TotalCopies"], develoverSettings.OptionAutoNumericQuantity);

    function createModel() {
        model.documentTypeId = $("select#DocumentTypeId", $form).val();
        model.issueDate = $("input#IssueDate", $form).val();
        model.sequenceNo = $("input#SequenceNo", $form).val();
        model.departmentId = $("select#DepartmentId", $form).val();
        model.deliverId = $("select#DeliverId", $form).val();
        model.destination = $("input#Destination", $form).val();
        model.approverId = $("select#ApproverId", $form).val();
        model.signerId = $("select#SignerId", $form).val();

        model.documentSeq = $("input#DocumentSeq", $form).val();
        model.documentSymbol = $("input#DocumentSymbol", $form).val();
        model.documentNo = $("input#DocumentNo", $form).val();
        model.publishedDate = $("input#PublishedDate", $form).val();
        model.publishedPlaceId = $("select#PublishedPlaceId", $form).val();
        model.title = $("textarea#Title", $form).val();
        model.description = $("textarea#Description", $form).val();
        model.content = $("textarea#Content", $form).val();
        model.totalPages = TotalPages.getNumber();
        model.totalCopies = TotalCopies.getNumber();

        model.documentStatusId = $("select#DocumentStatusId", $form).val();
        model.documentStatusDate = $("input#DocumentStatusDate", $form).val();
        model.documentStatusNote = $("input#DocumentStatusNote", $form).val();

        let appendices = [];

        $tableAppendix.bootstrapTable("getData").map((row, index) => {
            let item = {
                id: row.id,
                ownerId: row.ownerId,
                issueDate: row.issueDate,
                appendixNo: row.appendixNo,
                title: row.title,
                description: row.description,
                attachmentId: row.attachmentId
            }

            appendices.push(item);
        });

        model.appendices = appendices;
    }

    this.save = () => {
        createModel();

        $.ajax({
            url: "/documents/outbox/save",
            type: "post",
            data: { mode, model }
        }).done((res) => {
            swal({
                title: "Success",
                text: "The document has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            }).then(() => {
                window.location.href = "/documents/outbox/detail?id=" + res.id;
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
            title: "Delete Document?",
            text: "Do you want to delete the document now? This cannot be undone.",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((res) => {
            if (res) {
                $.ajax({
                    url: "/documents/outbox/delete",
                    type: "post",
                    data: { id: model.id }
                }).done((res) => {
                    swal({
                        title: "Success",
                        text: "The document has been deleted!",
                        icon: "success",
                        timer: develoverSettings.swal.timer,
                        closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                        closeOnEsc: develoverSettings.swal.closeOnEsc
                    }).then(() => {
                        window.location.href = "/documents/outbox";
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
                $.ajax({
                    url: "/documents/outbox/cancel",
                    type: "post",
                    data: { mode, model }
                }).done((res) => {
                    if (mode === DocumentMode.EDIT) {
                        window.location.href = "/documents/outbox/detail?id=" + model.id;
                    }
                    else {
                        history.back();
                    }
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
        let columns = model.detailColumns;
        let appendixColumns = model.detailAppendixColumns;

        if (mode === DocumentMode.VIEW) {
            columns.pop(); // remove operate column
            appendixColumns.pop(); // remove operate column
            appendixColumns.pop(); // remove operate column
        }
        else {
            appendixColumns.splice(appendixColumns.length - 3, 1);
        }

        $('.summernote').summernote({ height: 200, focus: false });
        initBootstrapTable(options.tableId, columns, [], model.detailDataUrl);
        initBootstrapTable(options.tableAppendixId, appendixColumns, [], model.detailAppendixDataUrl);

        if (mode === DocumentMode.VIEW) {
            $form.find("input:not(.datepicker), textarea").prop("readonly", true);
            $form.find("input.datepicker, select, button, a.btn").prop("disabled", true);

            $('.summernote').summernote('disable');

            $form.find(".add-attachment").remove();
            $form.find(".add-appendix").remove();
        }

        $form.find("input#DocumentNo").prop("disabled", true);
    }


    $('select#DocumentTypeId', $form).on('change', GetDocumentSeq);
    $('input#IssueDate', $form).on('change', GetDocumentSeq);
    $('select#DepartmentId', $form).on('change', GetDocumentSeq);

    $('input#DocumentSymbol', $form).on('change', CombineDocumentNo);

    function GetDocumentSeq() {
        let id = $('input#Id').val();
        let typeId = $('select#DocumentTypeId').val();
        let issueDate = $('input#IssueDate').val();
        let departmentId = $('select#DepartmentId').val();

        if (!typeId || typeId == '' || issueDate == '' || !departmentId || departmentId == '')
            return;

        $.ajax({
            url: '/documents/outbox/getdocumentseq',
            type: 'get',
            data: { id, typeId, issueDate, departmentId }
        }).done((res) => {
            $('input#DocumentSeq').val(res);
            CombineDocumentNo();
        }).fail((err) => {
            console.log(err)
        });
    }

    function CombineDocumentNo() {
        let seq = $('input#DocumentSeq').val();
        let symbol = $('input#DocumentSymbol').val();

        $('input#DocumentNo').val(seq + '/' + symbol);
    }
}