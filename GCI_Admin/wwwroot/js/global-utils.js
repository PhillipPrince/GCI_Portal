// ===============================
// Convert Base64 Byte Array to Image Source
// Can be used anywhere globally
// ===============================
function setBase64Image(imgElementId, base64String) {

    if (!base64String) {
        console.warn("No image data provided.");
        return;
    }

    var imageElement = document.getElementById(imgElementId);

    if (!imageElement) {
        console.error("Image element not found: " + imgElementId);
        return;
    }

    // Convert byte array to base64 string if necessary
    if (Array.isArray(base64String)) {
        base64String = btoa(
            new Uint8Array(base64String)
                .reduce((data, byte) => data + String.fromCharCode(byte), '')
        );
    }

    imageElement.src = "data:image/jpeg;base64," + base64String;
}

// ==========================================
// Convert Image File To Base64 String
// Usage: pass file input element and callback
// ==========================================
function convertImageToBase64(fileInput, callback) {

    if (!fileInput || !fileInput.files || fileInput.files.length === 0) {
        console.warn("No file selected.");
        return;
    }

    var file = fileInput.files[0];

    // Optional: Validate file type
    if (!file.type.startsWith("image/")) {
        console.error("Selected file is not an image.");
        return;
    }

    var reader = new FileReader();

    reader.onload = function (e) {

        var base64String = e.target.result;

        // Remove metadata part (data:image/jpeg;base64,)
        var pureBase64 = base64String.split(',')[1];

        if (callback && typeof callback === "function") {
            callback(pureBase64);
        }
    };

    reader.onerror = function (error) {
        console.error("Error converting image:", error);
    };

    reader.readAsDataURL(file);
}

