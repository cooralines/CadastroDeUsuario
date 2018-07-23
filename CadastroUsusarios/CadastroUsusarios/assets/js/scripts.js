var btnNovo = document.getElementById('novo');
var btnSalvar = document.getElementById('btnSalvar');

btnNovo.onclick = function () {
    if ($.active)
        return;

    $.ajax({
        url: "/User/Novo",
        cache: false,
        method: "get",
        success: function (dados) {
            $("#vao").html(dados);

            $("#modalCriar").modal();

            $("#modalCriar").on("hidden.bs.modal", function () {
                $("#vao").empty();
            })
        },
        error: function () {
            alert("Deu erro");
        }
    });
}

function modalEditar(id) {
    var obj = {
        id: id
    };

    $.ajax({
        url: "/User/ModalEditar",
        cache: false,
        data: obj,
        method: "GET",
        success: function (dados) {
            $("#vao").html(dados);

            $("#modalEditar").modal({
                backdrop: "static",
                keyboard: false
            });
        },
        error: function () {
            alert("Deu erro");
        }
    });
}

function modalExcluir(id) {
    var usuario = {
        id: id
    };

    idExcluir = id;

    $.ajax({
        url: "/User/ModalExcluir",
        cache: false,
        data: usuario,
        method: "GET",
        success: function (dados) {
            $("#vao").html(dados);

            $("#modalExcluir").modal({
                backdrop: "static",
                keyboard: false
            });
        },
        error: function () {
            alert("Deu erro");
        }
    });

}