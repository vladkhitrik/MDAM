$(document).ready(function () {

    var dlg = $("#dialog");

    dlg.dialog({
        autoOpen: false,
        modal: true,
        show: 'clip',
        hide: 'clip',
        position: {
            my: "center top",
            at: "center top+100",
        },
        resizable: false,
        draggable: false,
        buttons: [
            {
                id: "okBtn",
                click: function () {
                    $("#" + dlg.dialog("option", "form")).ajaxForm({
                        target: "#" + dlg.dialog("option", "form"),
                        success: function (data) {
                            var validationSummary = $('.validation-summary-errors ul li');
                            if (validationSummary.length == 0) {
                                dlg.dialog("close");
                                UpdateNavbar();
                            }
                        }
                    }).submit();
                }
            },
            {
                text: "Отмена",
                click: function () { dlg.dialog("close") }
            }
        ]
    });

    $("body").on("click", ".viewDialog", function (e) {
        e.preventDefault();

        if (dlg.dialog("isOpen")) {
            if (dlg.dialog("option", "title") === $(this).attr("dialogTitle")) {
                dlg.dialog("close");
            }
            else {
                dlg.load(this.href, function () { $.validator.unobtrusive.parse("#" + dlg.dialog("option", "form")); });
            }
        }
        else {
            dlg.load(this.href, function () { dlg.dialog("open"); $.validator.unobtrusive.parse("#" + dlg.dialog("option", "form")); });
        }

        dlg.dialog({
            title: $(this).attr("dialogTitle"),
            form: $(this).attr("dialogForm")
        });
        $("#okBtn").button("option", "label", $(this).attr("okBtn"));
    });
    dlg.keypress(function (e) {
        if (e.keyCode == $.ui.keyCode.ENTER) {
            $("#okBtn").click();
        }
    });
});