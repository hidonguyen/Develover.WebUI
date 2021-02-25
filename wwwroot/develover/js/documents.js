const DocumentMode = Object.freeze({ "NEW": 1, "EDIT": 2, "VIEW": 3 });

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