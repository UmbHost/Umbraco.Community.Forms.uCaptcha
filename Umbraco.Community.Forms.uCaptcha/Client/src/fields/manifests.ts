const uCaptchaFieldPreviewManifest = {
    type: "formsFieldPreview",
    alias: "uCaptcha.Field.Preview",
    name: "uCaptcha Preview",
    element: () => import('./ucaptcha-field-preview.js')
};

export const manifests = [uCaptchaFieldPreviewManifest];