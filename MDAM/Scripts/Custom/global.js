$(document).ready(function () {

    $.ajaxSetup({ cache: false });

    $("body").on("click", "#logoutLink", function () {
        $('#logoutForm').ajaxForm({ success: function () { UpdateNavbar(); } }).submit();
    })
});