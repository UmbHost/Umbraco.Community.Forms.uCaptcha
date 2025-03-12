import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
    LitElement,
    css,
    customElement,
    html
} from "@umbraco-cms/backoffice/external/lit";

const elementName = "ucaptcha-field-preview";

@customElement(elementName)
export class uCaptchaFieldPreviewElement extends UmbElementMixin(LitElement) {

    render() {
        return html`<img src="/App_Plugins/UmbracoCommunityFormsuCaptcha/images/uCaptcha.png" style="max-height: 90px;" />`;
    }

    static styles = css`
  `;
}

export default uCaptchaFieldPreviewElement;

declare global {
    interface HTMLElementTagNameMap {
        [elementName]: uCaptchaFieldPreviewElement;
    }
}