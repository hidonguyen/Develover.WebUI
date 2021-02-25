$(function () {
    "use strict"
    let isZKeyDown = false;

    $(document).on("keydown", (e) => {
        if (isZKeyDown)
            e.preventDefault();

        if (["Z", "z"].includes(e.originalEvent.key)) {
            isZKeyDown = true;
        }
    });

    $(document).on("keyup", (e) => {
        if (isZKeyDown) {
            if (e.originalEvent.key) {
                let hotkey = e.originalEvent.key.toUpperCase();

                if (hotkey === "B") {
                    history.back();
                }
                else {
                    let target = $("[data-hotkey='Z+" + hotkey + "']").last();

                    if (target[0])
                        target[0].click();
                }
            }
        }

        if (isZKeyDown && ["Z", "z"].includes(e.originalEvent.key)) {
            e.preventDefault();
            isZKeyDown = false;
        }
    });
});