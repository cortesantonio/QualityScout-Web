function switchView(view) {
  views = ["dashboard", "usuarios", "productos", "busqueda"];
  for (i = 0; i < views.length; i++) {
    pagina = document.getElementById(views[i]);
    pagina.style.display = "none";
    if (view == views[i]) {
      pagina.style.display = "block";
    }
  }
  localStorage.setItem("view", view);
}


if(localStorage.getItem('view') != null){
    switchView(localStorage.getItem('view'))
} else {
    switchView("dashboard")

}
// Para recuperar el estado desde sessionStorage

function showOptionUser() {
    var opciones = document.getElementById('opcionesUser');
    if (opciones.style.display == 'block') {
        opciones.style.display = 'none';
    } else {
        opciones.style.display = 'block';
    }
}
var openModalBtn = document.getElementById("openModalBtn");
var modal = document.getElementById("myModal");
var closeModalBtn = document.getElementById("closeModalBtn");
var scannerInitialized = false;

// Función para inicializar el scanner
function initializeScanner() {
    Quagga.init({
        inputStream: {
            name: "Live",
            type: "LiveStream",
            target: document.querySelector("#camera"),
            constraints: {
                width: 640,
                height: 480,
                facingMode: "environment",
                aspectRatio: { min: 1, max: 2 },
            },
            singleChannel: false,
        },
        locator: {
            patchSize: "medium",
            halfSample: true,
        },
        numOfWorkers: navigator.hardwareConcurrency || 4,
        frequency: 10,
        decoder: {
            readers: ["ean_reader"],
        },
        locate: true,
        multiple: false,
        locate: true,
        debug: {
            drawBoundingBox: true,
            showFrequency: true,
            drawScanline: true,
            showPattern: true
        },
    }, function (err) {
        if (err) {
            console.log(err);
            return;
        }
        console.log("Inicialización completada. Listo para empezar");

        Quagga.start();
    });

    Quagga.onDetected((result) => {
        const barcode = result.codeResult.code;

        Quagga.stop();

        var inputBusqueda = document.getElementById("Busqueda");
        inputBusqueda.value = barcode;

        document.getElementById("formularioBuscar").submit();
    });
}

// Abrir el modal y inicializar el scanner al hacer clic en el botón
openModalBtn.onclick = function () {
    modal.style.display = "block";
    if (!scannerInitialized) {
        initializeScanner();
        scannerInitialized = true;
    }
};

// Cerrar el modal y detener el scanner al hacer clic en la "X"
closeModalBtn.onclick = function () {
    modal.style.display = "none";
    if (scannerInitialized) {
        Quagga.stop();
        scannerInitialized = false;
    }
};

// También puedes cerrar el modal haciendo clic fuera de él
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
        if (scannerInitialized) {
            Quagga.stop();
            scannerInitialized = false;
        }
    }
};
