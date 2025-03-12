export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Umbraco Community Formsu Captcha Entrypoint",
    alias: "Umbraco.Community.Forms.uCaptcha.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint"),
  }
];
