$(function () {
    $(".confirm").confirm({
        title: "Вы действительно уверены?",
        text: "Данное действие нельзя будет отменить.",
        confirmButton: "Да",
        cancelButton: "Нет",
        post: true,
    });
});