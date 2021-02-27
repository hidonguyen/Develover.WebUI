var employee = (function (self) {
    let baseUrl = '/employee';
    let mode = CatalogMode.VIEW;
    let modelId = urlParams.get('id');
    let context = $("#panel-employee");

    let isModal = false;
    if ($("#modal-employee").length) {
        context = $("#modal-employee");
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
        $('#dt-employees').dataTable({
            responsive: true,
            ajax: baseUrl + '/getlist',
            columns: [
                { data: "code", title: "Mã" },
                { data: "refCode", title: "Mã tham chiếu" },
                { data: "firstName", title: "Tên riêng" },
                { data: "middleName", title: "Tên đệm" },
                { data: "lastName", title: "Tên họ" },
                { data: "email", title: "Thư điện tử" },
                { data: "phone", title: "Điện thoại" },
                { data: "alternativePhone", title: "Điện thoại thay thế" },
                { data: "division", title: "Khu vực" },
                { data: "position", title: "Vị trí" },
                { data: "hireDate", title: "Ngày thuê" },
                { data: "originalHireDate", title: "Ngày thuê gốc" },
                { data: "terminationDate", title: "Ngày kết thúc hợp đồng" },
                { data: "terminationReason", title: "Lí do kết thúc hợp đồng" },
                { data: "reHireDate", title: "Ngày thuê lại" },
                { data: "avatar", title: "Ảnh đại diện" },
                { data: "identificationNo", title: "Nhận dạng" },
                { data: "personalTaxCode", title: "Mã số thuế cá nhân" },
                { data: "dateOfBirth", title: "Ngày sinh" },
                { data: "placeOfBirth", title: "Nơi sinh" },
                { data: "gender", title: "Giới tính" },
                { data: "maritalStatus", title: "Tình trạng hôn nhân" },
                { data: "ethnicGroup", title: "Dân tộc" },
                { data: "permanentAddress", title: "Địa chỉ thường trú" },
                { data: "temporaryAddress", title: "Địa chỉ tạm trú" },
                { data: "socialInsuranceNo", title: "Bảo hiểm xã hội" },
                { data: "socialInsuranceDate", title: "Ngày bảo hiểm xã hội" },
                { data: "healthInsuranceNo", title: "Bảo hiểm y tế" },
                { data: "healthInsuranceDate", title: "Ngày bảo hiểm y tế" },
                { data: "note", title: "Ghi chú" },
                { data: "status", title: "Tình trạng" }
            ],
            columnDefs: [
                {
                    targets: 0,
                    render: employeeDrillDownFormatter
                },
                {
                    targets: 10,
                    render: dateFormatter
                },
                {
                    targets: 11,
                    render: dateFormatter
                },
                {
                    targets: 12,
                    render: dateFormatter
                },
                {
                    targets: 14,
                    render: dateFormatter
                },
                {
                    targets: 16,
                    render: dateFormatter
                },
                {
                    targets: 26,
                    render: dateFormatter
                },
                {
                    targets: 28,
                    render: dateFormatter
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
        $("#Code", context).val(model.code);
        $("#RefCode", context).val(model.refCode);
        $("#FirstName", context).val(model.firstName);
        $("#MiddleName", context).val(model.middleName);
        $("#LastName", context).val(model.lastName);
        $("#Email", context).val(model.email);
        $("#Phone", context).val(model.phone);
        $("#AlternativePhone", context).val(model.alternativePhone);
        $("#Division", context).val(model.division);
        $("#Position", context).val(model.position);
        $("#HireDate", context).val(model.hireDate);
        $("#OriginalHireDate", context).val(model.originalHireDate);
        $("#TerminationDate", context).val(model.terminationDate);
        $("#TerminationReason", context).val(model.terminationReason);
        $("#ReHireDate", context).val(model.reHireDate);
        $("#Avatar", context).val(model.avatar);
        $("#IdentificationNo", context).val(model.identificationNo);
        $("#PersonalTaxCode", context).val(model.personalTaxCode);
        $("#DateOfBirth", context).val(model.dateOfBirth);
        $("#PlaceOfBirth", context).val(model.placeOfBirth);
        $("#Gender", context).val(model.gender);
        $("#MaritalStatus", context).val(model.maritalStatus);
        $("#EthnicGroup", context).val(model.ethnicGroup);
        $("#PermanentAddress", context).val(model.permanentAddress);
        $("#TemporaryAddress", context).val(model.temporaryAddress);
        $("#SocialInsuranceNo", context).val(model.socialInsuranceNo);
        $("#SocialInsuranceDate", context).val(model.socialInsuranceDate);
        $("#HealthInsuranceNo", context).val(model.healthInsuranceNo);
        $("#HealthInsuranceDate", context).val(model.healthInsuranceDate);
        $("#Note", context).val(model.note);
        $("#Status", context).prop('checked', model.status);
    }

    let createModel = () => {
        model.code = $("input#Code", context).val();
        model.refCode = $("input#RefCode", context).val();
        model.firstName = $("input#FirstName", context).val();
        model.middleName = $("input#MiddleName", context).val();
        model.lastName = $("input#LastName", context).val();
        model.email = $("input#Email", context).val();
        model.phone = $("input#Phone", context).val();
        model.alternativePhone = $("input#AlternativePhone", context).val();
        model.division = $("input#Division", context).val();
        model.position = $("input#Position", context).val();
        model.hireDate = $("input#HireDate", context).val();
        model.originalHireDate = $("input#OriginalHireDate", context).val();
        model.terminationDate = $("input#TerminationDate", context).val();
        model.terminationReason = $("input#TerminationReason", context).val();
        model.reHireDate = $("input#ReHireDate", context).val();
        model.avatar = $("input#Avatar", context).val();
        model.identificationNo = $("input#IdentificationNo", context).val();
        model.personalTaxCode = $("input#PersonalTaxCode", context).val();
        model.dateOfBirth = $("input#DateOfBirth", context).val();
        model.placeOfBirth = $("input#PlaceOfBirth", context).val();
        model.gender = $("input#Gender", context).val();
        model.maritalStatus = $("input#MaritalStatus", context).val();
        model.ethnicGroup = $("input#EthnicGroup", context).val();
        model.permanentAddress = $("textarea#PermanentAddress", context).val();
        model.temporaryAddress = $("textarea#TemporaryAddress", context).val();
        model.socialInsuranceNo = $("input#SocialInsuranceNo", context).val();
        model.socialInsuranceDate = $("input#SocialInsuranceDate", context).val();
        model.healthInsuranceNo = $("input#HealthInsuranceNo", context).val();
        model.healthInsuranceDate = $("input#HealthInsuranceDate", context).val();
        model.note = $("textarea#Note", context).val();
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
    employee.initialize();
}); 