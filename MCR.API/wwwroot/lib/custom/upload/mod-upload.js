(function ($) {
    $.fn.customDropzone = function (options) {
        var defaults = {
            url: "/upload",
            maxFiles: 10,
            maxFilesize: 10,
            acceptedFiles: "image/*",
            addRemoveLinks: true,
            parallelUploads: 1,
            uploadMultiple: false,
            autoQueue: true,
            disablePreviews: true,
            clickable: true,
            onSending: function (file) { },
            onQueueComplete: function (dropzone) { },
            onSuccess: function (file, res) { },
            onError: function (file, errorMessage) { }
        };
        var settings = $.extend({}, defaults, options);

        return this.each(function () {
            var $this = $(this);
            var isButton = $this.is('button');
            var $container;

            // Verifica se já existe uma instância do Dropzone
            if ($this.data('dropzone-initialized')) {
                console.warn('Dropzone já inicializado para este elemento. Ignorando nova inicialização.');
                return;
            }

            // Limpa eventos e elementos antigos, se existirem
            if ($this.data('dropzone')) {
                $this.data('dropzone').destroy();
            }

            if (isButton) {
                $container = $this.parent('.dropzone-container');
                if (!$container.length) {
                    $container = $('<div class="dropzone-container"></div>');
                    $this.wrap($container);
                    $container = $this.parent();
                }
            } else {
                $container = $this;
            }

            var dropzoneConfig = {
                url: settings.url,
                maxFiles: settings.maxFiles,
                maxFilesize: settings.maxFilesize,
                acceptedFiles: settings.acceptedFiles,
                addRemoveLinks: settings.addRemoveLinks,
                parallelUploads: settings.parallelUploads,
                uploadMultiple: settings.uploadMultiple,
                autoQueue: settings.autoQueue,
                disablePreviews: settings.disablePreviews,
                clickable: isButton ? $this[0] : settings.clickable
            };

            var dropzone = new Dropzone($container[0], dropzoneConfig);

            dropzone.on("sending", settings.onSending);
            dropzone.on("queuecomplete", function () {
                settings.onQueueComplete(dropzone);
                dropzone.removeAllFiles(true);
            });
            dropzone.on("success", settings.onSuccess);
            dropzone.on("error", settings.onError);

            $container.data('dropzone', dropzone);
            $this.data('dropzone-initialized', true);

            if (isButton) {
                $this.off('click').on('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                });
            }
        });
    };
}(jQuery));

// Exemplo de uso
/*
$(document).ready(function () {
    $("#btUploadJson").customDropzone({
        url: "/Files/UploadJson",
        maxFiles: 1,
        maxFilesize: 5, // 5 MB
        acceptedFiles: "application/json",
        addRemoveLinks: true,
        autoQueue: true,
        onSending: function (file) {
            console.log("Enviando arquivo: " + file.name);
        },
        onQueueComplete: function (progress, res) {
            console.log("Upload completo!");
        },
        onSuccess: function (file, res) {
            if (res.success) {
                console.log("Arquivo JSON enviado com sucesso: " + res.url);
                // Você pode adicionar aqui qualquer ação adicional após o upload bem-sucedido
            } else {
                console.error("Erro no upload: " + res.message);
                notify.error("Erro no Upload", res.message);
            }
        },
        onError: function (file, errorMessage) {
            console.error("Erro no upload do arquivo " + file.name + ": " + errorMessage);
            notify.error("Erro no Upload", errorMessage);
        }
    });
});
*/