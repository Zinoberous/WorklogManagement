function initializeEditorTabKeyHandling(editorElement) {
    editorElement.addEventListener('keydown', function (e) {
        if (e.key === 'Tab') {
            e.preventDefault();
            document.execCommand('insertText', false, '\t');
        }
    });
}

function insertEditorHtml(editorElement, html) {
    // Erstelle das neue HTML-Element
    const tempDiv = document.createElement('div');
    tempDiv.innerHTML = html;

    const newElement = tempDiv.firstChild;

    // Finde den Hauptcontainer
    const contentContainer = editorElement.querySelector('.rz-html-editor-content');

    // Ermittle die aktuelle Cursorposition
    const selection = window.getSelection();

    let range;

    try {
        range = selection.getRangeAt(0);
    } catch {
        range = null;
    }

    let currentElement = range ? range.startContainer : null;

    // Falls der Cursor nicht im Editor ist oder die Range ungültig ist
    if (!currentElement || !contentContainer.contains(currentElement)) {
        contentContainer.appendChild(newElement);
    }
    else {
        // Iteriere nach oben, um das nächste Top-Level-Element zu finden
        while (currentElement && currentElement.parentElement !== contentContainer) {
            currentElement = currentElement.parentElement;
        }

        // Füge das neue HTML nach dem aktuellen Top-Level-Element ein
        if (currentElement.nextSibling) {
            contentContainer.insertBefore(newElement, currentElement.nextSibling);
        } else {
            contentContainer.appendChild(newElement);
        }
    }

    // Positioniere den Cursor hinter dem <br>-Tag
    const brElement = newElement.querySelector('br');
    if (brElement) {
        const newRange = document.createRange();
        newRange.setStartAfter(brElement);
        newRange.collapse(true);

        selection.removeAllRanges();
        selection.addRange(newRange);
    }

    // Setze den Fokus zurück auf den Editor
    editorElement.focus();
}
