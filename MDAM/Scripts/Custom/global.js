$(document).ready(function () {

    $.ajaxSetup({ cache: false });

    $("body").on("click", "#logoutLink", function () {
        $('#logoutForm').ajaxForm({ success: function () { UpdateNavbar(); } }).submit();
    });
});
$(document).ajaxStop(function () {
    $.unblockUI();
}).ajaxStart(function () {
    $.blockUI({
        message: "Загрузка...",
        theme: false,
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: 'rgba(0,0,0,0.6)',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            'border-radius': '20px',
            color: '#fff'
        },
        onBlock: function () {
            $("body").css({ overflow: 'hidden' })
        },
        onUnblock: function () {
            $("body").css({ overflow: 'inherit' })
        }
    });
});