{
    "use strict"

    function saveauthority(context, type, id) {
        let model = {};
        let permissions = [];

        $('.permission:checked', context).each(function () {
            let p = {};
            p.id = $(this).attr('id').substring(2);
            permissions.push(p);
        });

        model.type = type;
        model.id = id;
        model.permissions = permissions;

        $.ajax({
            url: '/identity/authority',
            type: 'post',
            data: model
        }).done(function (response) {
            swal({
                title: "Success",
                text: "Authority has been saved!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            });
        }).fail(function (error) {
            swal('[' + error.status + '] ' + error.responseText, {
                icon: "error",
            });
        });
    }

}
