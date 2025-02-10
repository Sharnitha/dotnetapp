// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById("deployForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const branch = document.getElementById("branch").value;
    if (!branch) {
        alert("Please select a branch.");
        return;
    }

    const response = await fetch("/Deploy/RunPipeline", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
        },
        body: new URLSearchParams({ branch: branch })
    });

    if (response.ok) {
        alert("Pipeline triggered successfully!");
    } else {
        alert("Failed to trigger pipeline.");
    }
});
