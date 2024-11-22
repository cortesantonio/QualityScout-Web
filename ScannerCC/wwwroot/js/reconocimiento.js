async function detectObjects() {
    let loading = true;
    const infoDiv = document.getElementById('informacion');

    if (loading) {

        infoDiv.innerHTML = `
            <p>Cargando...</p>
        `
    }

    const video = document.getElementById('video');
    const canvas = document.getElementById('canvas');
    const ctx = canvas.getContext('2d');
    let prediccionesHechas = [];
    // Cargar el modelo preentrenado COCO-SSD
    const model = await cocoSsd.load();

    try {
        // Obtener el stream de la cámara
        const stream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: { exact: "environment" } }
        });
        video.srcObject = stream;

    }
    
    catch {
        // Obtener el stream de la cámara
        const stream = await navigator.mediaDevices.getUserMedia({
            video: {  }
        });
        video.srcObject = stream;

    }




    // Esperar a que la cámara cargue y realizar la detección en bucle
    video.addEventListener('loadeddata', async () => {
        loading = false;
        infoDiv.innerHTML = ``
        while (true) {
            // Realizar la detección de objetos
            const predictions = await model.detect(video);

            // Dibujar las predicciones en el canvas
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            predictions.forEach(prediction => {
                ctx.beginPath();
                ctx.rect(
                    prediction.bbox[0],
                    prediction.bbox[1],
                    prediction.bbox[2],
                    prediction.bbox[3]
                );
                ctx.lineWidth = 1;
                ctx.strokeStyle = 'green';
                ctx.fillStyle = 'green';
                ctx.stroke();
                ctx.fillText(
                    `${prediction.class} (${Math.round(prediction.score * 100)}%)`,
                    prediction.bbox[0],
                    prediction.bbox[1] > 10 ? prediction.bbox[1] - 5 : 10
                );



                // Mostrar dimensiones estimadas en el HTML si el objeto es una botella


                //mostrar las preddiciones en una lista
                if (!prediccionesHechas.includes(prediction.class)) {
                    prediccionesHechas.push(prediction.class);
                }



                infoDiv.innerHTML = `
                            <p>${prediccionesHechas.join(', ')}</p>
                        `
            });

            // Esperar un breve periodo antes de la próxima detección
            await new Promise(resolve => setTimeout(resolve, 100)); // 100 ms
        }
    });
}

function copiarEnPortaPapeles() {
    const infoDiv = document.getElementById('informacion');
    const texto = infoDiv.innerText;
    navigator.clipboard.writeText(texto);
    alert("Texto copiado al portapapeles: " + texto);
}

function TerminarBusqueda() {
    copiarEnPortaPapeles()

    const video = document.getElementById('video');
    const canvas = document.getElementById('canvas');
    const ctx = canvas.getContext('2d');
    const infoDiv = document.getElementById('informacion');
    const boton = document.getElementById('boton');

    // Detener la cámara
    const stream = video.srcObject;
    const tracks = stream.getTracks();
    tracks.forEach(track => track.stop());

    // Limpiar el canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Limpiar el contenido del div de información
    infoDiv.innerHTML = '';

    // Limpiar el array de predicciones
    prediccionesHechas = [];

    // Reiniciar el botón
    boton.disabled = false;
    boton.innerHTML = 'Iniciar búsqueda';
    boton.onclick = function () {
        location.reload();
    };

}


// Iniciar la detección de objetos cuando la ventana haya cargado completamente
window.onload = detectObjects;