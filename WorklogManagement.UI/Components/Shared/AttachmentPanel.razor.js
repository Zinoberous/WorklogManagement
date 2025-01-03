function downloadFile(fileName, base64Data) {
    const blob = new Blob([Uint8Array.from(atob(base64Data), c => c.charCodeAt(0))]);
    const url = URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;

    document.body.appendChild(link);
    link.click();

    document.body.removeChild(link);
    URL.revokeObjectURL(url);
}
