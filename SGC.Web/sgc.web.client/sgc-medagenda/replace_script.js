import { readFileSync, writeFileSync } from "fs";

const path = "src/app/(auth)/login/page.tsx";
let code = readFileSync(path, "utf-8");

// Adding state
code = code.replace(
  "const [showPassword, setShowPassword] = useState(false);",
  `const [showPassword, setShowPassword] = useState(false);
  const [isAdminLogin, setIsAdminLogin] = useState(false);
  
  const bgGrad = isAdminLogin ? "from-indigo-900 via-indigo-800 to-indigo-600" : "from-[#064e3b] via-[#065f46] to-[#059669]";
  const shadowColor = isAdminLogin ? "shadow-indigo-500/20" : "shadow-emerald-500/20";
  const textColor = isAdminLogin ? "text-indigo-200" : "text-emerald-200";
  const rightBgGrad = isAdminLogin ? "from-indigo-50/50 dark:from-indigo-950/20" : "from-emerald-50/50 dark:from-emerald-950/20";
  const focusRing = isAdminLogin ? "focus:ring-indigo-500/30 focus:border-indigo-50" : "focus:ring-emerald-500/30 focus:border-emerald-500";
  const textLink = isAdminLogin ? "text-indigo-600 dark:text-indigo-400 hover:text-indigo-700 dark:hover:text-indigo-300" : "text-emerald-600 dark:text-emerald-400 hover:text-emerald-700 dark:hover:text-emerald-300";
  const btnGrad = isAdminLogin ? "from-indigo-500 to-indigo-600 hover:from-indigo-600 hover:to-indigo-700 shadow-indigo-500/25 hover:shadow-indigo-500/30" : "from-emerald-500 to-emerald-500 hover:from-emerald-600 hover:to-emerald-600 shadow-emerald-500/25 hover:shadow-emerald-500/30";
`
);

// Toggle UI
const toggleUI = `
            {/* Toggle Role */}
            <div className="flex bg-secondary p-1 rounded-xl mb-6">
              <button
                type="button"
                onClick={() => setIsAdminLogin(false)}
                className={\`flex-1 py-2 text-sm font-medium rounded-lg transition-all \${!isAdminLogin ? 'bg-background shadow-sm text-foreground' : 'text-muted-foreground hover:text-foreground'}\`}
              >
                Paciente
              </button>
              <button
                type="button"
                onClick={() => setIsAdminLogin(true)}
                className={\`flex-1 py-2 text-sm font-medium rounded-lg transition-all \${isAdminLogin ? 'bg-background shadow-sm text-foreground' : 'text-muted-foreground hover:text-foreground'}\`}
              >
                Administrador
              </button>
            </div>
`;

code = code.replace(
  "{/* Error */}",
  toggleUI + "\n            {/* Error */}"
);

// Left panel bg
code = code.replace(
  "from-[#064e3b] via-[#065f46] to-[#059669]",
  "${bgGrad}"
);
code = code.replace(
  "className=\"hidden lg:flex lg:w-1/2 relative overflow-hidden\n                      bg-gradient-to-br ${bgGrad}\"",
  "className={`hidden lg:flex lg:w-1/2 relative overflow-hidden bg-gradient-to-br ${bgGrad} transition-colors duration-500`}"
);

code = code.replace(
  "shadow-emerald-500/20",
  "${shadowColor}"
);
code = code.replace(
  "className=\"w-24 h-24 bg-white/10 rounded-3xl flex items-center\n                            justify-center mx-auto mb-6 backdrop-blur-sm\n                            border border-white/20 shadow-2xl\n                            ${shadowColor}\"",
  "className={`w-24 h-24 bg-white/10 rounded-3xl flex items-center justify-center mx-auto mb-6 backdrop-blur-sm border border-white/20 shadow-2xl transition-all duration-500 ${shadowColor}`}"
);

code = code.replace(
  "text-emerald-200 text-lg mb-12",
  "${textColor} text-lg mb-12 transition-colors duration-500"
);
code = code.replace(
  "className=\"text-emerald-200 text-lg mb-12\"",
  "className={`text-lg mb-12 transition-colors duration-500 ${textColor}`}"
);
code = code.replace(
  "className=\"${textColor} text-lg mb-12 transition-colors duration-500\"",
  "className={`text-lg mb-12 transition-colors duration-500 ${textColor}`}"
);


code = code.replace(
  "from-emerald-50/50\n                        to-transparent dark:from-emerald-950/20",
  "${rightBgGrad}"
);
code = code.replace(
  "className=\"absolute inset-0 bg-gradient-to-br ${rightBgGrad} pointer-events-none\"",
  "className={`absolute inset-0 bg-gradient-to-br pointer-events-none transition-colors duration-500 ${rightBgGrad}`}"
);

// Form Inputs
code = code.replace(
  "focus:ring-emerald-500/30 focus:border-emerald-500",
  "${focusRing}"
).replace(
  "focus:ring-emerald-500/30 focus:border-emerald-500",
  "${focusRing}"
);
code = code.replace(
  "className=\"w-full pl-10 pr-4 py-3 bg-secondary border\n                               border-border rounded-xl text-sm\n                               focus:outline-none focus:ring-2\n                               ${focusRing}\n                               transition-all placeholder:text-muted-foreground/50\"",
  "className={`w-full pl-10 pr-4 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 transition-all placeholder:text-muted-foreground/50 ${focusRing}`}"
);
code = code.replace(
  "className=\"w-full pl-10 pr-12 py-3 bg-secondary border\n                               border-border rounded-xl text-sm\n                               focus:outline-none focus:ring-2\n                               ${focusRing}\n                               transition-all placeholder:text-muted-foreground/50\"",
  "className={`w-full pl-10 pr-12 py-3 bg-secondary border border-border rounded-xl text-sm focus:outline-none focus:ring-2 transition-all placeholder:text-muted-foreground/50 ${focusRing}`}"
);

// Links
code = code.replace(
  "text-emerald-600 dark:text-emerald-400\n                               hover:underline font-medium",
  "${textLink} hover:underline font-medium"
);
code = code.replace(
  "className=\"${textLink} hover:underline font-medium\"",
  "className={`text-xs hover:underline font-medium transition-colors ${textLink}`}"
);

// Submit Buttons
code = code.replace(
  "from-emerald-500\n                           to-emerald-500 hover:from-emerald-600 hover:to-emerald-600",
  "${btnGrad}"
);
code = code.replace(
  "shadow-emerald-500/25\n                           hover:shadow-xl hover:shadow-emerald-500/30",
  ""
);
code = code.replace(/className="submit-btn w-full bg-gradient-to-r \${btnGrad}[^"]*"/s, "className={`submit-btn w-full bg-gradient-to-r text-white font-semibold py-3 rounded-xl transition-all duration-500 text-sm mt-2 disabled:opacity-60 disabled:cursor-not-allowed flex items-center justify-center gap-2 shadow-lg ${btnGrad}`}");

// Bottom link
code = code.replace(
  "text-emerald-600 dark:text-emerald-400 hover:text-emerald-700\n                             dark:hover:text-emerald-300 font-semibold\n                             hover:underline transition-colors",
  "${textLink} font-semibold hover:underline"
);
code = code.replace(
  "className=\"${textLink} font-semibold hover:underline\"",
  "className={`font-semibold hover:underline transition-colors duration-500 ${textLink}`}"
);

writeFileSync(path, code);
console.log("Replaced");
