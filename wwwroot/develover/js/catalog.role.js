var role = (function (self) {
    let baseUrl = '/role';
    let mode = CatalogMode.VIEW;
    let modelId = urlParams.get('id');
    let context = $("#panel-role");

    let isModal = false;
    if ($("#modal-role").length) {
        context = $("#modal-role");
        isModal = true;
    }

    let isInDetailPage = false;
    if (!isModal && context.length > 0) {
        isInDetailPage = true;
    }

    let isSaved = false;

    let model = {};

    let postbackId = "";
    let postbackClass = "";

    let initializeIndex = () => {
        $('#dt-roles').dataTable({
            responsive: true,
            ajax: baseUrl + '/getlist',
            columns: [
                { data: "name", title: "Tên" },
                { data: "sequenceNo", title: "Trình tự" },
                { data: "description", title: "Mô tả" },
                { data: "status", title: "Tình trạng" }
            ],
            columnDefs: [
                {
                    targets: 0,
                    render: roleDrillDownFormatter
                },
                {
                    targets: -1,
                    width: 65,
                    render: statusFormatter
                }
            ],
            dom:
                "<'row mb-3'<'col-sm-12 col-md-6 d-flex flex-wrap align-items-center justify-content-start'<'custom-button mb-1 mr-3'>f><'col-sm-12 col-md-6 d-flex align-items-center justify-content-end'<'mr-3'l>B>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
            buttons: [
                {
                    extend: "excelHtml5",
                    text: '<i class="fal fa-file-excel"></i>',
                    titleAttr: "Export to Excel",
                    className: "btn-outline-primary btn-sm mr-1"
                },
                //{
                //    extend: "copyHtml5",
                //    text: '<i class="fal fa-copy"></i>',
                //    titleAttr: "Copy to clipboard",
                //    className: "btn-outline-primary btn-sm mr-1"
                //},
                {
                    extend: "print",
                    text: '<i class="fal fa-print"></i>',
                    titleAttr: "Print table",
                    className: "btn-outline-primary btn-sm"
                }
            ]
        });

        $("div.custom-button").html('<a class="btn btn-new-index btn-outline-success" title="Thêm mới (Z+N)" href="javascript:void(0);" data-hotkey="Z+N"><i class="fal fa-file-plus"></i><span> Thêm mới</span></a>');
        $('.btn-new-index').on('click', self.new);
    }

    self.addButtonEvents = () => {
        $('.btn-new', context).on('click', self.new);
        $('.btn-edit', context).on('click', self.edit);
        $('.btn-save', context).on('click', self.save);
        $('.btn-cancel', context).on('click', self.cancel);
        $('.btn-delete', context).on('click', self.delete);
        $('.btn-close', context).on('click', self.hideModal);
    }

    let getModel = (id) => {
        $.ajax({
            url: baseUrl + "/getmodel?id=" + id,
            type: "get",
            async: false
        }).done((res) => {
            model = res.model;
        }).fail((err) => {
            console.log(err);
            showErrorMessage();
        });
    }

    let loadData = () => {
        getModel(modelId);
        modelId = model.id;
        if (isEmptyUUID(model.id)) {
            mode = CatalogMode.INVALID;
            setFunctionButtonState(mode, context);
            disableInputData(context);
            Swal.fire({
                title: "Dữ liệu không tồn tại",
                text: "Không tìm thấy thông tin, vui lòng kiểm tra lại!",
                icon: "error",
            });
        }
        clearInputData(context);
        //binding to control
        $("#Name", context).val(model.name);
        $("#SequenceNo", context).val(model.sequenceNo);
        $("#Description", context).val(model.description);
        $("#Status", context).prop('checked', model.status);
    }

    let createModel = () => {
        model.name = $("input#Name", context).val();
        model.sequenceNo = $("input#SequenceNo", context).val();
        model.description = $("textarea#Description", context).val();
        model.status = $("input#Status", context).prop("checked");
    }

    self.initialize = () => {
        self.addButtonEvents();
        if (isModal) {
            mode = CatalogMode.NEW;
            getModel();
        }
        else {
            if (isInDetailPage) {
                if (modelId && !isEmptyUUID(modelId)) {
                    mode = CatalogMode.VIEW;
                    loadData();
                    disableInputData(context);
                }
                else {
                    mode = CatalogMode.NEW;
                    getModel();
                }
                setFunctionButtonState(mode, context);
            }
            else {
                initializeIndex();
            }
        }
    }

    /** Modal */

    context.on("shown.bs.modal", (e) => {
        postbackId = $(e.relatedTarget).data("postback-id");
        postbackClass = $(e.relatedTarget).data("postback-class");

        mode = CatalogMode.NEW;
        getModel();
    })

    context.on("hidden.bs.modal", (e) => {
        postbackId = "";
        postbackClass = "";

        mode = CatalogMode.INVALID;
        clearInputData();
    })

    self.hideModal = () => {
        context.modal("hide");
    }

    /** Modal */

    self.new = () => {
        window.location.href = baseUrl + '/new';
    }

    self.edit = () => {
        if (mode === CatalogMode.INVALID)
            return;

        mode = CatalogMode.EDIT;
        setFunctionButtonState(mode, context);
        enableInputData(context);
    }

    self.save = () => {
        if (mode === CatalogMode.INVALID)
            return;

        if (!$("form", context)[0].checkValidity())
            return;

        createModel();

        console.log(model);

        //$.ajax({
        //    url: baseUrl + "/save",
        //    type: "post",
        //    data: { mode, model }
        //}).done((res) => {
        //    isSaved = true;
        //    if (isModal) {
        //        if (model.status) {
        //            $("select." + postbackClass).map((index, select) => {
        //                $(select).append("<option value=\"" + res.id + "\">" + res.name + "</option>");
        //                $(select).selectpicker("refresh");
        //            });

        //            $("#" + postbackId).selectpicker("val", res.id);
        //            $("button[data-id=\"" + postbackId + "\"]").attr("title", res.name);
        //            $(".filter-option-inner-inner", $("button[data-id=\"" + postbackId + "\"]")).text(res.name);
        //        }

        //        self.hideModal();
        //    }
        //    else {
        //        Swal.fire({
        //            title: "Success",
        //            text: "The branch has been saved!",
        //            icon: "success"
        //        }).then(() => {
        //            window.location.href = baseUrl + "/detail?id=" + res.id;
        //        })
        //    }
        //}).fail((err) => {
        //    console.log(err);
        //    if (err.status === 400) {
        //        showHideValidateResult(JSON.parse(err.responseText), context);
        //    }
        //    else {
        //        showErrorMessage();
        //    }
        //});
    }

    self.cancel = () => {
        if (mode === CatalogMode.INVALID)
            return;

        if (mode === CatalogMode.NEW && (!modelId || isEmptyUUID(modelId))) {
            history.back();
        }
        else {
            Swal.fire({
                title: "Huỷ phiếu?",
                text: "Dữ liệu bạn đã nhập sẽ bị mất, bạn có chắc chắn muốn huỷ phiếu không?",
                icon: "question",
                showCancelButton: true,
                confirmButtonText: "Có",
                cancelButtonText: "Không"
            }).then(function (result) {
                if (result.value) {
                    self.initialize();
                }
            });
        }
    }

    self.delete = () => {
        if (mode === CatalogMode.INVALID)
            return;

        Swal.fire({
            title: "Xoá phiếu?",
            text: "Dữ liệu bạn đã nhập sẽ bị xoá, bạn có chắc chắn muốn xoá phiếu không?",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Có",
            cancelButtonText: "Không"
        }).then((res) => {
            if (res.value) {
                //$.ajax({
                //    url: baseUrl + "/delete",
                //    type: "post",
                //    data: { id: model.id }
                //}).done((res) => {
                //    window.location.href = baseUrl;
                //}).fail((err) => {
                //    console.log(err);
                //    showErrorMessage();
                //});
            }
        });
    }

    //window.addEventListener('beforeunload', function (e) {
    //    if (!isModal && !isSaved && mode === CatalogMode.NEW || mode === CatalogMode.EDIT)
    //        e.returnValue = "";
    //});

    return self;
})({});

document.addEventListener('DOMContentLoaded', function () {
    role.initialize();
}); 