const fs = require('fs');
const path = require('path');
const adminDir = path.join('c:/Users/johan/source/repos/SGC/SGC.Web/sgc.web.client/sgc-medagenda/src/app/(admin)/admin');
const dirs = ['citas', 'usuarios', 'medicos', 'disponibilidad', 'especialidades', 'proveedores', 'pagos', 'auditoria', 'settings'];

dirs.forEach(d => {
  const file = path.join(adminDir, d, 'page.tsx');
  if (fs.existsSync(file)) {
    let content = fs.readFileSync(file, 'utf8');
    let changed = false;
    
    // Add import if not exists
    if (!content.includes('usePageTransition')) {
      content = content.replace(/(import .*;\n)/, '$1import { usePageTransition } from "@/components/animations/Animatedcomponents";\n');
      changed = true;
    }
    
    // Add usePageTransition call inside the component
    if (!content.match(/usePageTransition\(\);/)) {
      content = content.replace(/(export default function .*\(.*\) \{\n)/, '$1  usePageTransition();\n');
      changed = true;
    }
    
    // Add page-content to the main div
    if (!content.includes('page-content') && content.includes('className=')) {
      // Find the first return statement with div className
      content = content.replace(/(return \([\s\S]*?<div className="([^"]*)")/, (match, p1, p2) => {
        return `return (\\n    <div className="${p2} page-content"`;
      });
      changed = true;
    }
    
    if (changed) {
      fs.writeFileSync(file, content);
      console.log('Updated ' + d);
    } else {
      console.log('No changes needed for ' + d);
    }
  } else {
    console.log('File not found: ' + file);
  }
});
