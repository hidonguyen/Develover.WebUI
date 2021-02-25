{
    "use strict"

    function changepassword(context) {
        $.ajax({
            url: '/identity/changepassword',
            type: 'post',
            data: context.serialize()
        }).done(function (response) {
            swal({
                title: "Success",
                text: "Password has been changed!",
                icon: "success",
                timer: develoverSettings.swal.timer,
                closeOnClickOutside: develoverSettings.swal.closeOnClickOutside,
                closeOnEsc: develoverSettings.swal.closeOnEsc
            }).then(() => {
                window.location.href = "/identity/profile?id=" + response.id;
            })
        }).fail(function (error) {
            showHideValidateResult(JSON.parse(error.responseText), context);
        });
    }

}
