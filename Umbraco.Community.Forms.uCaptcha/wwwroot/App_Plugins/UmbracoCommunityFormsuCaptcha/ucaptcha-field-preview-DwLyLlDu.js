import { UmbElementMixin as i } from "@umbraco-cms/backoffice/element-api";
import { LitElement as p, html as c, css as o, customElement as u } from "@umbraco-cms/backoffice/external/lit";
var h = Object.getOwnPropertyDescriptor, v = (m, a, n, s) => {
  for (var e = s > 1 ? void 0 : s ? h(a, n) : a, r = m.length - 1, l; r >= 0; r--)
    (l = m[r]) && (e = l(e) || e);
  return e;
};
const g = "ucaptcha-field-preview";
let t = class extends i(p) {
  render() {
    return c`<img src="/App_Plugins/UmbracoCommunityFormsuCaptcha/images/uCaptcha.png" style="max-height: 90px;" />`;
  }
};
t.styles = o`
  `;
t = v([
  u(g)
], t);
const d = t;
export {
  d as default,
  t as uCaptchaFieldPreviewElement
};
//# sourceMappingURL=ucaptcha-field-preview-DwLyLlDu.js.map
