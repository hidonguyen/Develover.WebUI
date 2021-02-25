"use strict"

const urlParams = new URLSearchParams(window.location.search);

$('.datepicker').datepicker({
    autoclose: true,
    format: develoverSettings.formatDateDatepicker,
    todayBtn: 'linked',
    clearBtn: true,
    todayHighlight: true,
}).datepicker("setDate", 'now');

$('.select2').select2({
    placeholder: 'Vui lòng chọn giá trị',
    closeOnSelect: true,
    debug: true
})

//$('.datetimepicker').datetimepicker({
//    //sideBySide: true,
//    dayViewHeaderFormat: develoverSettings.formatDate,
//    format: develoverSettings.formatDate + " HH:mm:ss",
//    extraFormats: ['DD/MM/YYYYThh:mm:ss A'],
//    showTodayButton: true,
//    showClear: true,
//    showClose: true,
//    keepOpen: true,
//    toolbarPlacement: 'top',
//    minDate: moment().subtract(1, "days"),
//    useCurrent: true,
//    icons: {
//        time: 'fad fa-clock',
//        date: 'fad fa-calendar-star',
//        up: 'fad fa-chevron-up',
//        down: 'fad fa-chevron-down',
//        previous: 'fad fa-chevron-left',
//        next: 'fad fa-chevron-right',
//        today: 'fad fa-calendar-day',
//        clear: 'fad fa-trash',
//        close: 'fad fa-times'
//    },

//});

//$('.multiselect').multiselect({
//    inheritClass: true,
//    numberDisplayed: 0,
//    buttonContainer: '<div class="btn-group w-100 mw-100" />',
//    enableFiltering: true,
//    filterBehavior: 'text',
//    enableCaseInsensitiveFiltering: true
//});

//$('.dropify').dropify();

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}
function isEmptyUUID(uuid) {
    return uuid == '00000000-0000-0000-0000-000000000000';
}

var showHideValidateResult = (errors, context) => {
    errors.map((error, index) => {
        let fieldKey = error.key;
        let state = error.state;
        let message = error.message;

        let input = $('#' + fieldKey, context);
        if (state === 'Valid') {
            if (input.hasClass('selectpicker')) {
                input.closest('div.bootstrap-select').removeClass('is-invalid');
            }
            else {
                input.removeClass('is-invalid');
            }
            $('.invalid-feedback', input.closest('div.field-group')).html('');
        }
        else {
            if (input.hasClass('selectpicker')) {
                input.closest('div.bootstrap-select').addClass('is-invalid');
            }
            else {
                input.addClass('is-invalid');
            }
            $('.invalid-feedback', input.closest('div.field-group')).html(message);
        }
    })
}

var showErrorMessage = (title, text) => {
    Swal.fire({
        title: title ?? 'Có lỗi xảy ra',
        text: text ?? 'Vui lòng liên hệ bộ phận kỹ thuật!',
        icon: 'error'
    });
}

function statusFormatter(data, type, row, meta) {
    if (data) {
        return '<span class="badge badge-success">Sử dụng</span>';
    }
    else {
        return '<span class="badge badge-secondary">Không sử dụng</span>';
    }
}

function imageFormatter(value, row, index) {
    return '<img src="' + value + '" width="100" />';
}

function attachmentFormatter(value, row, index) {
    return [
        //'<a class="btn view text-success text-center px-0 py-0" href="javascript:void(0)" title="View">',
        //'<i class="far fa-eye"></i>',
        //'</a>  ',
        '<a class="btn download text-primary text-center px-0 py-0" href="javascript:void(0)" title="Download">',
        '<i class="far fa-cloud-download-alt"></i>',
        '</a>'
    ].join('');
}

function attachmentAppendixEditFormatter(value, row, index) {
    if (row.attachmentId && row.attachmentId === "00000000-0000-0000-0000-000000000000") {
        return [
            '<a class="btn upload text-warning text-center px-0 py-0" href="javascript:void(0)" title="Upload">',
            '<i class="far fa-upload"></i>',
            '</a>  '
        ].join('');
    }

    if (row.attachmentId && row.attachmentId !== "00000000-0000-0000-0000-000000000000") {
        return [
            '<a class="btn download text-primary text-center px-0 py-0" href="javascript:void(0)" title="Download">',
            '<i class="far fa-cloud-download-alt"></i>',
            '</a>',
            '<a class="btn upload text-warning text-center px-0 py-0" href="javascript:void(0)" title="Re-Upload">',
            '<i class="far fa-upload"></i>',
            '</a>',
            '<a class="btn delete text-danger text-center px-0 py-0" href="javascript:void(0)" title="Delete">',
            '<i class="far fa-times-circle"></i>',
            '</a>'
        ].join('');
    }
}

function attachmentAppendixViewFormatter(value, row, index) {
    if (row.attachmentId && row.attachmentId !== "00000000-0000-0000-0000-000000000000") {
        return [
            '<a class="btn download text-primary text-center px-0 py-0" href="javascript:void(0)" title="Download">',
            '<i class="far fa-cloud-download-alt"></i>',
            '</a>'
        ].join('');
    }

    return '';
}

function operateFormatter(value, row, index) {
    return [
        '<a class="btn edit text-warning text-center px-0 py-0" href="javascript:void(0)" title="Edit">',
        '<i class="fal fa-edit"></i>',
        '</a>  ',
        '<a class="btn delete text-danger text-center px-0 py-0" href="javascript:void(0)" title="Delete">',
        '<i class="fal fa-times-circle"></i>',
        '</a>'
    ].join('');
}

function seqNoFormatter(data, type, row, meta) {
    return ++meta.row;
}

function seqNoPidFormatter(value, row, index) {
    if (row.pid === "")
        return ++index;
    else

        return "";
}

function dateFormatter(data, type, row, meta) {
    if (!data) return data;

    let dateFormat = ['YYYY-MM-DDThh:mm:ss'];

    if (moment(data, dateFormat).isValid()) {
        if (moment(data, dateFormat).year() === 1900)
            return "";
        else
            return moment(data, dateFormat).format(develoverSettings.formatDate);
    }
    else
        return data;
}

function numberFormatterPacking(value, row, index) {
    if (!isNaN(parseFloat(value))) {
        return accounting.formatNumber(value, develoverSettings.roundPacking, develoverSettings.thousandsSymbol, develoverSettings.decimalSymbol);
    }
    else {
        return value;
    }
}

function numberFormatterQuantity(value, row, index) {
    if (!isNaN(parseFloat(value))) {
        return accounting.formatNumber(value, develoverSettings.roundQuantity, develoverSettings.thousandsSymbol, develoverSettings.decimalSymbol);
    }
    else {
        return value;
    }
}

function numberFormatterAmount(value, row, index) {
    if (!isNaN(parseFloat(value))) {
        return accounting.formatNumber(value, develoverSettings.roundAmount, develoverSettings.thousandsSymbol, develoverSettings.decimalSymbol);
    }
    else {
        return value;
    }
}

function numberFormatterAmountVND(value, row, index) {
    if (!isNaN(parseFloat(value))) {
        return accounting.formatNumber(value, develoverSettings.roundAmountVND, develoverSettings.thousandsSymbol, develoverSettings.decimalSymbol);
    }
    else {
        return value;
    }
}

function summaryFooterFormatterPacking(data) {
    let field = this.field
    let summary = data.map(function (row) {
        if (row['pid'] == '') {
            return 0;
        } else {
            return +accounting.unformat(row[field]);
        }
    }).reduce(function (sum, i) {
        return sum + i
    }, 0)

    return accounting.formatNumber(summary, develoverSettings.roundPacking, develoverSettings.thousandsSymbol, develoverSettings.decimalSymbol);
}

function summaryFooterFormatterQuantity(data) {
    let field = this.field
    let summary = data.map(function (row) {
        if (row['pid'] == '') {
            return 0;
        } else {
            return +accounting.unformat(row[field]);
        }
    }).reduce(function (sum, i) {
        return sum + i
    }, 0)

    return accounting.formatNumber(summary, develoverSettings.roundQuantity, develoverSettings.thousandsSymbol, develoverSettings.decimalSymbol);
}

function summaryFooterFormatterAmount(data) {
    let field = this.field
    let summary = data.map(function (row) {
        if (row['pid'] == '') {
            return 0;
        } else {
            return +accounting.unformat(row[field]);
        }
    }).reduce(function (sum, i) {
        return sum + i
    }, 0)

    return accounting.formatNumber(summary, develoverSettings.roundAmount, develoverSettings.thousandsSymbol, develoverSettings.decimalSymbol);
}

function summaryFooterFormatterAmountVND(data) {
    let field = this.field
    let summary = data.map(function (row) {
        if (row['pid'] == '') {
            return 0;
        } else {
            return +accounting.unformat(row[field]);
        }
    }).reduce(function (sum, i) {
        return sum + i
    }, 0)

    return accounting.formatNumber(summary, develoverSettings.roundAmountVND, develoverSettings.thousandsSymbol, develoverSettings.decimalSymbol);
}




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
function setDateNull(fieldId, context) {
    fieldId.map((field, index) => {
        let input = $('#' + field, context);
        if (input.val() === null || input.val() === '') {
            input.datepicker('update', moment().format(develoverSettings.formatDate));
        }
    });
}

function clearInputData(context) {
    $("input[type=\"text\"], input[type=\"email\"], input[type=\"password\"], textarea", context).val("");
    $("select.select2", context).val(null).trigger('change');
    $("input[type=\"radio\"], input[type=\"checkbox\"]", context).prop("checked", false);
    $("input[type=\"checkbox\"]#Status", context).prop("checked", true);
}

function enableInputData(context) {
    $("input:not(.datepicker), textarea", context).prop("readonly", false);
    $("input.datepicker, select, button:not(.btn-panel), a, input[type=\"radio\"], input[type=\"checkbox\"]", context).prop("disabled", false);
}

function disableInputData(context) {
    $("input:not(.datepicker), textarea", context).prop("readonly", true);
    $("input.datepicker, select, button:not(.btn-panel), a, input[type=\"radio\"], input[type=\"checkbox\"]", context).prop("disabled", true);
}