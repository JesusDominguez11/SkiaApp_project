window.flowerAnimation = {

    draw: function (canvasId) {

        const canvas = document.getElementById(canvasId);

        if (!canvas)
            return;


        const ctx = canvas.getContext("2d");


        let progress = 0;


        function animate() {

            ctx.clearRect(
                0,
                0,
                canvas.width,
                canvas.height);


            drawFlower(
                ctx,
                canvas.width / 2,
                canvas.height / 2,
                progress);


            progress += 0.01;


            if (progress <= 1)
                requestAnimationFrame(animate);
        }


        animate();
    }

};


function drawFlower(ctx, x, y, p) {

    const maxRadius = 40;

    const radius =
        maxRadius * p;


    if (radius <= 0)
        return;


    ctx.fillStyle = "hotpink";


    for (let i = 0; i < 8; i++) {

        const angle =
            i * Math.PI * 2 / 8;


        const px =
            x + Math.cos(angle) *
            radius * 1.5;


        const py =
            y + Math.sin(angle) *
            radius * 1.5;


        ctx.beginPath();

        ctx.arc(
            px,
            py,
            radius,
            0,
            Math.PI * 2);

        ctx.fill();
    }


    ctx.fillStyle = "gold";


    ctx.beginPath();

    ctx.arc(
        x,
        y,
        radius,
        0,
        Math.PI * 2);

    ctx.fill();
}