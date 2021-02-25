const VoucherMode = Object.freeze({ "INVALID": -1, "NEW": 1, "EDIT": 2, "VIEW": 3 });

function setFunctionButtonState(state, context) {
    switch (state) {
        case VoucherMode.INVALID: {
            $(".btn-new", context).show();
            $(".btn-edit", context).hide();
            $(".btn-save", context).hide();
            $(".btn-cancel", context).hide();
            $(".btn-delete", context).hide();
        } break;
        case VoucherMode.VIEW: {
            $(".btn-new", context).show();
            $(".btn-edit", context).show();
            $(".btn-save", context).hide();
            $(".btn-cancel", context).hide();
            $(".btn-delete", context).show();
        } break;
        case VoucherMode.NEW: {
            $(".btn-new", context).hide();
            $(".btn-edit", context).hide();
            $(".btn-save", context).show();
            $(".btn-cancel", context).show();
            $(".btn-delete", context).hide();
        } break;
        case VoucherMode.EDIT: {
            $(".btn-new", context).hide();
            $(".btn-edit", context).hide();
            $(".btn-save", context).show();
            $(".btn-cancel", context).show();
            $(".btn-delete", context).show();
        } break;
    }
}