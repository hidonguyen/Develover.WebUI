
function DeveloverSetting(options, model) {

    let $formLocalizationsave = $("#" + options.formLocalization);
    let $formSMTP = $("#" + options.formSMTP);

    [RoundPacking, RoundQuantity, RoundAmount, RoundAmountVND, SMTPPort, LenghtNo] = AutoNumeric.multiple(['input#RoundPacking',
        'input#RoundQuantity',
        'input#RoundAmount',
        'input#RoundAmountVND',
        'input#SMTPPort',
        'input#LenghtNo'], develoverSettings.OptionAutoNumericPacking);
    this.Localizationsave = () => {

        model.branchId = $("select#BranchId", $formLocalizationsave).val();
        model.currencyId = $("select#CurrencyId", $formLocalizationsave).val();
        model.formatDate = $("select#FormatDate", $formLocalizationsave).val();
        model.decimalSymbol = $("select#DecimalSymbol", $formLocalizationsave).val();
        model.thousandsSymbol = $("select#ThousandsSymbol", $formLocalizationsave).val();
        model.roundPacking = RoundPacking.getNumber();
        model.roundQuantity = RoundQuantity.getNumber();
        model.roundAmount = RoundAmount.getNumber();
        model.roundAmountVND = RoundAmountVND.getNumber();

        model.structureNo = $("input#StructureNo", $formLocalizationsave).val().toUpperCase();
        model.lenghtNo = $("input#LenghtNo", $formLocalizationsave).val();
        model.typeNo = $("select#TypeNo", $formLocalizationsave).val();

        $.ajax({
            url: "/setting/savedeveloversetting",
            type: "post",
            data: { model },
        }).done(function (response) {
            swal({
                title: "Success",
                text: "The setting has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            })
            showHideValidateResult(response.modelState, $formLocalizationsave);
        }).fail(function (err) {
            showHideValidateResult(err.responseJSON, $formLocalizationsave);
        });
    }


    this.SMTPsave = () => {

        model.emailFromAddress = $("input#EmailFromAddress").val();
        model.emailsFromName = $("input#EmailsFromName").val();
        model.smtpHost = $("input#SMTPHost").val();
        model.smtpPort = SMTPPort.getNumber();
        model.smtpUser = $("input#SMTPUser").val();
        model.smtpPassword = $("input#SMTPPassword").val();
        model.smtpSecurity = $("select#SMTPSecurity").val();
        model.smtpAuthenticationDomain = $("input#SMTPAuthenticationDomain").val();

        $.ajax({
            url: "/setting/savedeveloversetting",
            type: "post",
            data: { model },
        }).done(function (response) {
            swal({
                title: "Success",
                text: "The setting has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            })
            showHideValidateResult(response.modelState, $formLocalizationsave);
        }).fail(function (err) {
            showHideValidateResult(err.responseJSON, $formLocalizationsave);
        });
    }
} var showHideValidateResult = (err, context) => {
    err.map((error, index) => {
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
            $('.invalid-feedback', input.closest('div.form-group')).html('');
        }
        else {
            if (input.hasClass('selectpicker')) {
                input.closest('div.bootstrap-select').addClass('is-invalid');
            }
            else {
                input.addClass('is-invalid');
            }
            $('.invalid-feedback', input.closest('div.form-group')).html(message);
        }
    })
}