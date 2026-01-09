// Whiteboard JavaScript Module
let signaturePad = null;
let resizeObserver = null;
let inactivityTimer = null;
let inactivityCallback = null;

// Initialize the whiteboard
window.initializeWhiteboard = async function (canvas, options) {
    // Wait for SignaturePad to be available
    while (typeof SignaturePad === 'undefined') {
        await new Promise(resolve => setTimeout(resolve, 100));
    }

    // Apply background styling to canvas
    canvas.style.backgroundColor = '#ffffff';
    canvas.style.backgroundImage = 'url("/img/bg/rough_diagonal.webp")';
    canvas.style.backgroundPosition = 'center';
    canvas.style.backgroundRepeat = 'repeat';

    // Initialize SignaturePad
    signaturePad = new SignaturePad(canvas, {
        penColor: options.penColor || '#000000',
        backgroundColor: 'transparent', // Make background transparent to show our custom background
        minWidth: options.minWidth || 0.5,
        maxWidth: options.maxWidth || 2.5,
        throttle: options.throttle || 16
    });

    // Set up inactivity detection
    if (options.inactivityCallback) {
        inactivityCallback = options.inactivityCallback;
        setupInactivityDetection(canvas, options.inactivityTimeout || 10000); // Default 10 seconds
    }

    console.log('SignaturePad initialized with options:', options);
};

// Resize the whiteboard canvas
window.resizeWhiteboardCanvas = function (canvas, container) {
    if (!signaturePad) {
        console.warn('SignaturePad not initialized');
        return { width: 0, height: 0 };
    }

    // Save current strokes
    const data = signaturePad.toData();
    const oldWidth = canvas.width;
    const oldHeight = canvas.height;

    // Get container dimensions
    const width = container.clientWidth;
    const height = container.clientHeight;

    // Set canvas dimensions to match container
    canvas.width = width;
    canvas.height = height;

    // Clear and restore strokes with scaling
    signaturePad.clear();
    
    if (data && data.length > 0) {
        try {
            const scaleRatioX = width / oldWidth;
            const scaleRatioY = height / oldHeight;
            
            const scaledData = data.map(stroke => ({
                ...stroke,
                points: stroke.points.map(point => ({
                    x: point.x * scaleRatioX,
                    y: point.y * scaleRatioY,
                    pressure: point.pressure,
                    time: point.time
                }))
            }));
            
            signaturePad.fromData(scaledData);
        } catch (error) {
            console.warn('Could not restore strokes:', error);
        }
    }

    console.log(`Canvas resized: ${width}×${height}`);

    return {
        width: width,
        height: height
    };
};

// Clear the canvas
window.clearWhiteboardCanvas = function (canvas) {
    if (signaturePad) {
        signaturePad.clear();
        console.log('Canvas cleared');
    }
};

// Save canvas as PNG
window.saveWhiteboardCanvas = function (canvas) {
    if (!signaturePad) {
        console.warn('SignaturePad not initialized');
        return null;
    }

    // Create a new canvas for the final image
    const outputCanvas = document.createElement('canvas');
    const ctx = outputCanvas.getContext('2d');
    
    // Set the output canvas size to match the original
    outputCanvas.width = canvas.width;
    outputCanvas.height = canvas.height;
    
    // Fill with white background first
    ctx.fillStyle = '#ffffff';
    ctx.fillRect(0, 0, outputCanvas.width, outputCanvas.height);
    
    // Draw the signature pad strokes on top
    ctx.drawImage(canvas, 0, 0);
    
    // Generate the final data URL with white background
    const dataURL = outputCanvas.toDataURL('image/png');
    console.log('Canvas data URL generated with white background');
    return dataURL;
};

// Update whiteboard settings
window.updateWhiteboardSettings = function (canvas, settings) {
    if (!signaturePad) {
        console.warn('SignaturePad not initialized');
        return;
    }

    if (settings.penColor !== undefined) {
        signaturePad.penColor = settings.penColor;
    }
    if (settings.minWidth !== undefined) {
        signaturePad.minWidth = settings.minWidth;
    }
    if (settings.maxWidth !== undefined) {
        signaturePad.maxWidth = settings.maxWidth;
    }

    console.log('Whiteboard settings updated:', settings);
};

// Setup resize observer
window.setupWhiteboardResize = function (canvas, container, dotNetHelper) {
    if (resizeObserver) {
        resizeObserver.disconnect();
    }

    resizeObserver = new ResizeObserver(() => {
        // Debounce resize events
        clearTimeout(window.resizeTimeout);
        window.resizeTimeout = setTimeout(() => {
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OnCanvasResize');
            }
        }, 100);
    });

    resizeObserver.observe(container);
    console.log('Resize observer setup complete');
};

// Prevent touch scroll on the whiteboard area
document.addEventListener('DOMContentLoaded', function() {
    document.addEventListener('touchmove', function(e) {
        if (e.target.closest('.canvas-container')) {
            e.preventDefault();
        }
    }, { passive: false });
});

// Setup inactivity detection
function setupInactivityDetection(canvas, timeoutMs) {
    function resetInactivityTimer() {
        if (inactivityTimer) {
            clearTimeout(inactivityTimer);
        }
        
        inactivityTimer = setTimeout(() => {
            if (inactivityCallback && signaturePad && !signaturePad.isEmpty()) {
                console.log('Inactivity detected, triggering auto-save');
                inactivityCallback.invokeMethodAsync('OnInactivityDetected');
            }
        }, timeoutMs);
    }

    // Reset timer on any drawing activity
    signaturePad.addEventListener('beginStroke', resetInactivityTimer);
    signaturePad.addEventListener('beforeUpdateStroke', resetInactivityTimer);
    signaturePad.addEventListener('afterUpdateStroke', resetInactivityTimer);
    signaturePad.addEventListener('endStroke', resetInactivityTimer);

    // Also reset on mouse/touch events
    canvas.addEventListener('mousedown', resetInactivityTimer);
    canvas.addEventListener('touchstart', resetInactivityTimer);

    // Initial timer
    resetInactivityTimer();
}

// Cleanup function
window.cleanupWhiteboard = function () {
    if (inactivityTimer) {
        clearTimeout(inactivityTimer);
        inactivityTimer = null;
    }
    if (resizeObserver) {
        resizeObserver.disconnect();
        resizeObserver = null;
    }
    if (signaturePad) {
        signaturePad.off();
        signaturePad = null;
    }
    console.log('Whiteboard cleanup complete');
};
