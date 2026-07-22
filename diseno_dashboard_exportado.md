# Exportación del Diseño del Dashboard de Blazor

Este archivo contiene todo el diseño visual (CSS) y estructural (Razor HTML) listo para ser copiado a tu otro proyecto.

---

## 1. Archivo de Cabecera (HTML/Head)

Agrega estas importaciones en el bloque `<head>` del archivo principal del nuevo proyecto (ej. `Components/App.razor` o `index.html` / `_Host.cshtml`):

```html
<!-- Importar la tipografía Plus Jakarta Sans -->
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,200..800;1,200..800&display=swap" rel="stylesheet">

<!-- Vincular la hoja de estilo del dashboard -->
<link rel="stylesheet" href="css/dashboard.css" />
```

---

## 2. Hoja de Estilo Completa (`dashboard.css`)

Crea un archivo llamado `dashboard.css` dentro de la carpeta `wwwroot/css/` de tu nuevo proyecto y pega el siguiente código:

```css
/* Custom Dashboard Styling - Blazor Model Dashboard */

:root {
    --bg-main: #EAEAED;
    --bg-card: #FFFFFF;
    --bg-sidebar: #F4F5F6;
    --neon-accent: #D4F953;
    --neon-accent-hover: #C2E842;
    --text-primary: #1C1E21;
    --text-secondary: #5E6973;
    --text-muted: #8E9A8E;
    --border-color: rgba(220, 224, 228, 0.7);
    --shadow-soft: 0 8px 30px rgba(0, 0, 0, 0.03), 0 1px 3px rgba(0, 0, 0, 0.02);
    --shadow-card: 0 10px 40px -10px rgba(0, 0, 0, 0.05), 0 1px 2px rgba(0, 0, 0, 0.02);
    --font-family: 'Plus Jakarta Sans', system-ui, -apple-system, sans-serif;
    --transition-smooth: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
}

* {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
}

body {
    background-color: var(--bg-main);
    color: var(--text-primary);
    font-family: var(--font-family);
    overflow-x: hidden;
}

/* Dashboard Container Layout */
.dashboard-root {
    display: flex;
    min-height: 100vh;
    background-color: var(--bg-main);
    width: 100%;
}

/* Sidebar Styling */
.custom-sidebar {
    width: 80px;
    background-color: var(--bg-sidebar);
    border-right: 1px solid var(--border-color);
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-between;
    padding: 24px 0;
    position: sticky;
    top: 0;
    height: 100vh;
    z-index: 100;
    transition: var(--transition-smooth);
}

.logo-container {
    margin-bottom: 30px;
}

.logo-g {
    width: 44px;
    height: 44px;
    background: linear-gradient(135deg, #1C1E21 0%, #343A40 100%);
    border-radius: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 22px;
    font-weight: 800;
    color: var(--neon-accent);
    box-shadow: 0 4px 12px rgba(212, 249, 83, 0.25);
    cursor: pointer;
    transition: var(--transition-smooth);
    border: 1px solid rgba(212, 249, 83, 0.2);
    text-decoration: none;
}

.logo-g:hover {
    transform: scale(1.05) rotate(-5deg);
    box-shadow: 0 6px 18px rgba(212, 249, 83, 0.4);
}

.sidebar-menu {
    display: flex;
    flex-direction: column;
    gap: 16px;
    flex-grow: 1;
    justify-content: center;
    width: 100%;
    align-items: center;
}

.menu-item {
    width: 46px;
    height: 46px;
    border-radius: 12px;
    display: flex;
    align-items: center;
    justify-content: center;
    color: var(--text-secondary);
    background: transparent;
    border: none;
    cursor: pointer;
    position: relative;
    transition: var(--transition-smooth);
}

.menu-item svg {
    width: 20px;
    height: 20px;
    fill: currentColor;
    transition: var(--transition-smooth);
}

.menu-item:hover {
    color: var(--text-primary);
    background-color: rgba(0, 0, 0, 0.04);
    transform: translateY(-2px);
}

.menu-item.active {
    color: var(--text-primary);
    background-color: rgba(0, 0, 0, 0.06);
    box-shadow: inset 0 1px 2px rgba(0,0,0,0.05);
}

.menu-item.active::before {
    content: '';
    position: absolute;
    left: 0;
    top: 12px;
    bottom: 12px;
    width: 4px;
    background-color: var(--text-primary);
    border-radius: 0 4px 4px 0;
}

.sidebar-footer {
    display: flex;
    flex-direction: column;
    gap: 20px;
    align-items: center;
}

.footer-btn {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: var(--text-secondary);
    cursor: pointer;
    border: none;
    background: transparent;
    transition: var(--transition-smooth);
}

.footer-btn svg {
    width: 20px;
    height: 20px;
    fill: currentColor;
}

.footer-btn:hover {
    color: var(--text-primary);
    background-color: rgba(0, 0, 0, 0.04);
}

.avatar-container {
    width: 42px;
    height: 42px;
    border-radius: 50%;
    overflow: hidden;
    border: 2px solid white;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.08);
    cursor: pointer;
    transition: var(--transition-smooth);
    display: flex;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, #FF9A9E 0%, #FECFEF 99%, #FECFEF 100%);
}

.avatar-container svg {
    width: 28px;
    height: 28px;
    fill: #FFFFFF;
}

.avatar-container img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.avatar-container:hover {
    transform: scale(1.08);
    border-color: var(--neon-accent);
    box-shadow: 0 4px 12px rgba(212, 249, 83, 0.3);
}

/* Main Dashboard Area */
.main-content {
    flex-grow: 1;
    padding: 30px 40px;
    display: flex;
    flex-direction: column;
    gap: 24px;
    overflow-y: auto;
    max-height: 100vh;
}

/* Top bar navigation tabs */
.top-navigation {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
}

.nav-tabs-wrapper {
    display: flex;
    gap: 8px;
    background-color: rgba(0, 0, 0, 0.03);
    padding: 4px;
    border-radius: 12px;
    border: 1px solid var(--border-color);
}

.tab-btn {
    border: none;
    background: transparent;
    padding: 8px 18px;
    border-radius: 8px;
    font-size: 14px;
    font-weight: 600;
    color: var(--text-secondary);
    cursor: pointer;
    transition: var(--transition-smooth);
}

.tab-btn:hover {
    color: var(--text-primary);
    background-color: rgba(255, 255, 255, 0.5);
}

.tab-btn.active {
    background-color: #FFFFFF;
    color: var(--text-primary);
    box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}

.new-chat-btn {
    border: none;
    background-color: var(--neon-accent);
    color: var(--text-primary);
    font-weight: 700;
    font-size: 14px;
    padding: 10px 20px;
    border-radius: 10px;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 6px;
    box-shadow: 0 4px 14px rgba(212, 249, 83, 0.25);
    transition: var(--transition-smooth);
    border: 1px solid rgba(212, 249, 83, 0.1);
    text-decoration: none;
}

.new-chat-btn:hover {
    background-color: var(--neon-accent-hover);
    transform: translateY(-1px);
    box-shadow: 0 6px 18px rgba(212, 249, 83, 0.4);
}

.new-chat-btn svg {
    width: 14px;
    height: 14px;
    stroke: currentColor;
    stroke-width: 2.5;
}

/* Dashboard Grid System */
.dashboard-grid {
    display: grid;
    grid-template-columns: 3.8fr 6.2fr;
    gap: 24px;
    width: 100%;
}

.grid-col {
    display: flex;
    flex-direction: column;
    gap: 24px;
}

/* Premium Card Base */
.premium-card {
    background: #FFFFFF;
    border-radius: 24px;
    border: 1px solid var(--border-color);
    padding: 28px;
    box-shadow: var(--shadow-card);
    transition: var(--transition-smooth);
    position: relative;
    overflow: hidden;
}

.premium-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 15px 45px -10px rgba(0, 0, 0, 0.07), 0 2px 4px rgba(0, 0, 0, 0.01);
}

/* Animations definition */
@keyframes fadeInUp {
    from {
        opacity: 0;
        transform: translateY(20px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.animate-card {
    animation: fadeInUp 0.6s cubic-bezier(0.16, 1, 0.3, 1) both;
}

.delay-1 { animation-delay: 0.1s; }
.delay-2 { animation-delay: 0.2s; }
.delay-3 { animation-delay: 0.3s; }
.delay-4 { animation-delay: 0.4s; }
.delay-5 { animation-delay: 0.5s; }

/* Call Recording Card Specifics */
.recording-card {
    background: radial-gradient(circle at top left, rgba(212, 249, 83, 0.12) 0%, rgba(255, 255, 255, 0.98) 60%), #FFFFFF;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    gap: 20px;
    min-height: 490px;
}

.recording-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.recording-title {
    font-size: 18px;
    font-weight: 700;
    color: var(--text-primary);
}

.recording-id-pill {
    display: flex;
    align-items: center;
    gap: 6px;
    background-color: rgba(0, 0, 0, 0.04);
    padding: 5px 12px;
    border-radius: 20px;
    font-size: 12px;
    font-weight: 600;
    color: var(--text-secondary);
}

.recording-id-pill svg {
    width: 12px;
    height: 12px;
    cursor: pointer;
    transition: var(--transition-smooth);
}

.recording-id-pill svg:hover {
    color: var(--text-primary);
    transform: scale(1.1);
}

.profile-section {
    display: flex;
    align-items: center;
    gap: 20px;
    margin: 15px 0;
}

.profile-img-container {
    width: 100px;
    height: 100px;
    border-radius: 50%;
    overflow: hidden;
    border: 3px solid white;
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
    background: linear-gradient(135deg, #84fab0 0%, #8fd3f4 100%);
    display: flex;
    align-items: center;
    justify-content: center;
}

.profile-img-container svg {
    width: 50px;
    height: 50px;
    fill: #FFFFFF;
}

.profile-img-container img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.profile-info h3 {
    font-size: 20px;
    font-weight: 700;
    color: var(--text-primary);
    margin-bottom: 4px;
}

.profile-info p {
    font-size: 13px;
    color: var(--text-secondary);
}

/* Sentiment Pills in Recording Card */
.sentiment-pills {
    display: flex;
    gap: 12px;
    margin: 10px 0;
}

.sentiment-pill {
    flex: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 10px 6px;
    border-radius: 12px;
    border: 1px solid var(--border-color);
    background-color: rgba(255, 255, 255, 0.5);
    cursor: pointer;
    transition: var(--transition-smooth);
}

.sentiment-pill-icon {
    width: 22px;
    height: 34px;
    border-radius: 16px;
    border: 1.5px solid var(--text-secondary);
    display: flex;
    align-items: center;
    justify-content: center;
    margin-bottom: 6px;
    position: relative;
    transition: var(--transition-smooth);
}

.sentiment-pill-icon::after {
    content: '';
    position: absolute;
    width: 6px;
    height: 6px;
    border-radius: 50%;
    background-color: var(--text-secondary);
    transition: var(--transition-smooth);
}

.sentiment-pill.positive .sentiment-pill-icon::after { top: 6px; }
.sentiment-pill.neutral .sentiment-pill-icon::after { top: 13px; }
.sentiment-pill.negative .sentiment-pill-icon::after { bottom: 6px; }

.sentiment-pill span {
    font-size: 11px;
    font-weight: 600;
    color: var(--text-secondary);
}

.sentiment-pill:hover {
    background-color: rgba(255, 255, 255, 0.9);
    border-color: #A0A5A8;
}

/* Active sentiment pill styles based on mockup */
.sentiment-pill.active {
    background-color: #FFFFFF;
    border-color: var(--text-primary);
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
}

.sentiment-pill.active span {
    color: var(--text-primary);
}

.sentiment-pill.active .sentiment-pill-icon {
    border-color: var(--neon-accent-hover);
    background-color: rgba(212, 249, 83, 0.1);
}

.sentiment-pill.active .sentiment-pill-icon::after {
    background-color: #8EB314;
    box-shadow: 0 0 6px var(--neon-accent);
}

/* Timeline/Audio Wave Representation */
.audio-timeline-section {
    display: flex;
    flex-direction: column;
    gap: 8px;
    margin-top: 10px;
}

.wave-display {
    height: 55px;
    background-color: rgba(0, 0, 0, 0.03);
    border-radius: 14px;
    position: relative;
    overflow: hidden;
    display: flex;
    align-items: center;
    border: 1px dashed rgba(0,0,0,0.05);
    cursor: pointer;
}

.wave-curtain-passed {
    position: absolute;
    left: 0;
    top: 0;
    bottom: 0;
    background-color: rgba(212, 249, 83, 0.15);
    border-right: 2px solid var(--text-primary);
    transition: width 0.1s linear;
    z-index: 1;
}

.wave-handle-dot {
    position: absolute;
    right: -6px;
    top: 50%;
    transform: translateY(-50%);
    width: 12px;
    height: 12px;
    border-radius: 50%;
    background-color: var(--text-primary);
    border: 2px solid var(--neon-accent);
    cursor: pointer;
    box-shadow: 0 0 6px rgba(0,0,0,0.2);
}

.wave-bars-container {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
    height: 100%;
    padding: 0 16px;
    z-index: 2;
    pointer-events: none;
}

.wave-bar {
    width: 3px;
    background-color: rgba(0, 0, 0, 0.15);
    border-radius: 3px;
    transition: var(--transition-smooth);
}

.wave-curtain-passed ~ .wave-bars-container .wave-bar.passed {
    background-color: var(--text-primary);
}

.time-labels {
    display: flex;
    justify-content: space-between;
    font-size: 11px;
    font-weight: 600;
    color: var(--text-secondary);
}

/* Audio Player Bottom Controls */
.audio-controls {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-top: 10px;
}

.playback-btns {
    display: flex;
    align-items: center;
    gap: 8px;
}

.playback-btn {
    width: 38px;
    height: 38px;
    border-radius: 10px;
    border: 1px solid var(--border-color);
    background-color: #FFFFFF;
    color: var(--text-primary);
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: var(--transition-smooth);
}

.playback-btn svg {
    width: 14px;
    height: 14px;
    fill: currentColor;
}

.playback-btn:hover {
    background-color: rgba(0, 0, 0, 0.03);
    transform: scale(1.05);
}

.playback-btn.play-pause {
    width: 48px;
    height: 48px;
    border-radius: 14px;
    background-color: #FFFFFF;
    border-color: var(--text-primary);
}

.playback-btn.play-pause:hover {
    background-color: rgba(0,0,0,0.02);
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.playback-btn.play-pause.playing svg {
    color: var(--text-primary);
}

.volume-control {
    display: flex;
    align-items: center;
    gap: 10px;
    flex-grow: 1;
    margin: 0 24px;
}

.volume-slider-track {
    flex-grow: 1;
    height: 4px;
    background-color: rgba(0, 0, 0, 0.1);
    border-radius: 2px;
    position: relative;
    cursor: pointer;
}

.volume-slider-fill {
    height: 100%;
    background-color: var(--text-primary);
    border-radius: 2px;
    width: 70%;
    position: relative;
}

.volume-slider-fill::after {
    content: '';
    position: absolute;
    right: -4px;
    top: -3px;
    width: 10px;
    height: 10px;
    border-radius: 50%;
    background-color: var(--text-primary);
    border: 2px solid white;
}

/* Completed Tasks Card Specifics */
.completed-tasks-card {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 24px 28px;
}

.tasks-info {
    display: flex;
    flex-direction: column;
    gap: 4px;
}

.tasks-title {
    font-size: 15px;
    font-weight: 700;
    color: var(--text-secondary);
}

.tasks-count-row {
    display: flex;
    align-items: baseline;
    gap: 12px;
}

.tasks-number {
    font-size: 40px;
    font-weight: 800;
    color: var(--text-primary);
    line-height: 1;
}

.tasks-trend {
    font-size: 12px;
    font-weight: 700;
    color: var(--text-secondary);
    display: flex;
    align-items: center;
    gap: 2px;
}

.tasks-trend.positive {
    color: #5E7A0C;
}

/* Vertical glowing bar animation chart */
.tasks-chart {
    display: flex;
    align-items: flex-end;
    gap: 6px;
    height: 60px;
}

.task-chart-bar {
    width: 8px;
    background: linear-gradient(180deg, rgba(212, 249, 83, 0.1) 0%, rgba(212, 249, 83, 0.6) 100%);
    border-radius: 4px;
    transition: height 1s cubic-bezier(0.16, 1, 0.3, 1);
    height: 10px; /* default, overwritten by style */
    position: relative;
    cursor: pointer;
}

.task-chart-bar:hover {
    background: linear-gradient(180deg, var(--neon-accent) 0%, var(--neon-accent-hover) 100%);
    box-shadow: 0 0 10px var(--neon-accent);
}

.task-chart-bar.active {
    background: linear-gradient(180deg, var(--neon-accent) 0%, var(--neon-accent-hover) 100%);
    box-shadow: 0 0 8px rgba(212, 249, 83, 0.8);
}

/* Metric Row Sparkline Cards */
.metrics-row {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 24px;
}

.metric-card {
    padding: 24px;
    display: flex;
    flex-direction: column;
    gap: 16px;
}

.metric-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.metric-title {
    font-size: 14px;
    font-weight: 700;
    color: var(--text-secondary);
}

.metric-btn {
    border: none;
    background: transparent;
    color: var(--text-secondary);
    cursor: pointer;
    width: 24px;
    height: 24px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: var(--transition-smooth);
}

.metric-btn:hover {
    background-color: rgba(0,0,0,0.04);
    color: var(--text-primary);
}

.metric-btn svg {
    width: 14px;
    height: 14px;
    fill: currentColor;
}

.metric-trend {
    font-size: 11px;
    font-weight: 600;
    color: var(--text-secondary);
}

.metric-value-row {
    display: flex;
    justify-content: space-between;
    align-items: flex-end;
}

.metric-value {
    font-size: 34px;
    font-weight: 800;
    color: var(--text-primary);
    line-height: 1;
}

.sparkline-container {
    width: 90px;
    height: 36px;
}

.sparkline-svg {
    width: 100%;
    height: 100%;
    overflow: visible;
}

.sparkline-path {
    fill: none;
    stroke-width: 2.5;
    stroke-linecap: round;
    stroke-linejoin: round;
    stroke-dasharray: 200;
    stroke-dashoffset: 200;
    animation: drawLine 1.5s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

@keyframes drawLine {
    to {
        stroke-dashoffset: 0;
    }
}

.sparkline-dots {
    transition: var(--transition-smooth);
}

/* Visualization Row Layout */
.visuals-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 24px;
}

/* Talk / Listen Ratio Card */
.ratio-card {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    gap: 20px;
    min-height: 250px;
}

.ratio-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.ratio-title {
    font-size: 15px;
    font-weight: 700;
    color: var(--text-secondary);
}

.ratio-options-btn {
    border: none;
    background: transparent;
    color: var(--text-secondary);
    cursor: pointer;
    font-size: 18px;
    line-height: 1;
}

.ratio-value-center {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    gap: 2px;
}

.ratio-big-num {
    font-size: 44px;
    font-weight: 800;
    color: var(--text-primary);
    line-height: 1;
}

.ratio-subtext {
    font-size: 12px;
    font-weight: 600;
    color: var(--text-secondary);
}

.ratio-slider-container {
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.ratio-slider-track {
    height: 16px;
    background-color: rgba(0, 0, 0, 0.05);
    border-radius: 8px;
    position: relative;
    overflow: hidden;
    display: flex;
}

.ratio-slider-fill {
    height: 100%;
    background-color: var(--neon-accent);
    border-radius: 8px 0 0 8px;
    width: 75%;
    transition: width 1s cubic-bezier(0.16, 1, 0.3, 1);
    position: relative;
}

.ratio-slider-divider {
    width: 3px;
    height: 100%;
    background-color: #FFFFFF;
    position: absolute;
    right: 0;
    top: 0;
}

.ratio-labels {
    display: flex;
    justify-content: space-between;
    font-size: 12px;
    font-weight: 700;
    color: var(--text-primary);
}

.ratio-percentages {
    display: flex;
    justify-content: space-between;
    font-size: 12px;
    font-weight: 600;
    color: var(--text-secondary);
}

/* Topic Duration / Circular Gauge Card */
.gauge-card {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    min-height: 250px;
    position: relative;
}

.gauge-chart-container {
    position: relative;
    width: 180px;
    height: 100px;
    margin: 10px auto 0 auto;
    overflow: hidden;
}

.gauge-svg {
    width: 100%;
    height: 100%;
}

.gauge-bg {
    fill: none;
    stroke: rgba(0, 0, 0, 0.04);
    stroke-width: 10;
    stroke-linecap: round;
}

.gauge-fill {
    fill: none;
    stroke: url(#gaugeGradient);
    stroke-width: 10;
    stroke-linecap: round;
    stroke-dasharray: 220; /* Circumference of half circle r=70 is pi*70 = ~220 */
    stroke-dashoffset: 220;
    animation: drawGauge 1.5s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

@keyframes drawGauge {
    to {
        stroke-dashoffset: 66; /* 70% fill = 220 * 0.3 = 66 offset */
    }
}

.gauge-text-overlay {
    position: absolute;
    bottom: 5px;
    left: 0;
    right: 0;
    text-align: center;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.gauge-sub {
    font-size: 11px;
    font-weight: 600;
    color: var(--text-secondary);
}

.gauge-val {
    font-size: 28px;
    font-weight: 800;
    color: var(--text-primary);
    line-height: 1.1;
}

/* Sentiment Staircase Chart Card (Bottom) */
.sentiment-chart-card {
    display: flex;
    flex-direction: column;
    gap: 20px;
}

.sentiment-chart-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.sentiment-chart-title {
    font-size: 15px;
    font-weight: 700;
    color: var(--text-secondary);
}

.sentiment-chart-filter-btn {
    border: none;
    background: transparent;
    color: var(--text-secondary);
    cursor: pointer;
    display: flex;
    align-items: center;
}

.sentiment-chart-filter-btn svg {
    width: 16px;
    height: 16px;
    fill: currentColor;
}

.staircase-chart-container {
    display: flex;
    align-items: flex-end;
    justify-content: space-between;
    height: 170px;
    margin-top: 10px;
    position: relative;
    padding-left: 120px; /* Space for the left labels */
}

.staircase-labels-left {
    position: absolute;
    left: 0;
    bottom: 0;
    top: 0;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    padding: 10px 0;
}

.staircase-label-item {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 12px;
    font-weight: 600;
    color: var(--text-secondary);
    cursor: pointer;
    transition: var(--transition-smooth);
}

.staircase-label-item:hover {
    color: var(--text-primary);
}

.staircase-bullet {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background-color: var(--text-secondary);
}

.staircase-label-item.positive .staircase-bullet { background-color: var(--neon-accent); }
.staircase-label-item.negative .staircase-bullet { background-color: #C0C8CF; }
.staircase-label-item.neutral .staircase-bullet { background-color: #E2E5E8; }

.staircase-label-item.active {
    color: var(--text-primary);
    font-weight: 700;
}

.staircase-steps {
    display: flex;
    align-items: flex-end;
    gap: 20px;
    flex-grow: 1;
    height: 100%;
}

.staircase-step-wrapper {
    flex: 1;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: flex-end;
    height: 100%;
}

.staircase-step-bar {
    width: 100%;
    background-color: rgba(0, 0, 0, 0.04);
    border-radius: 12px 12px 0 0;
    transition: height 1s cubic-bezier(0.16, 1, 0.3, 1), background-color 0.3s ease, box-shadow 0.3s ease;
    cursor: pointer;
    position: relative;
}

.staircase-step-bar:hover {
    background-color: rgba(0, 0, 0, 0.08);
}

/* Active coloring for staircase elements based on percentages */
.staircase-step-bar.positive-step {
    border-bottom: 4px solid var(--neon-accent);
}

.staircase-step-bar.positive-step.active {
    background: linear-gradient(180deg, rgba(212, 249, 83, 0.15) 0%, rgba(212, 249, 83, 0.02) 100%);
    border: 1.5px solid var(--neon-accent);
    border-bottom: 4px solid var(--neon-accent);
}

.staircase-step-bar.negative-step.active {
    background-color: rgba(0, 0, 0, 0.05);
    border: 1.5px solid #A0A5A8;
}

.staircase-step-bar.neutral-step.active {
    background-color: rgba(0, 0, 0, 0.02);
    border: 1.5px solid #D2D6D9;
}

.staircase-step-val {
    font-size: 26px;
    font-weight: 800;
    color: var(--text-primary);
    margin-bottom: 4px;
}

.staircase-step-label {
    position: absolute;
    top: -24px;
    left: 0;
    font-size: 11px;
    font-weight: 700;
    color: var(--text-secondary);
    white-space: nowrap;
}

/* Responsiveness adjustments */
@media (max-width: 1024px) {
    .dashboard-grid {
        grid-template-columns: 1fr;
    }
}

@media (max-width: 768px) {
    .metrics-row {
        grid-template-columns: 1fr;
    }
    .visuals-row {
        grid-template-columns: 1fr;
    }
    .main-content {
        padding: 20px;
    }
    .custom-sidebar {
        display: none; /* In production a hamburger menu, for mockup we keep it simple */
    }
}
```

---

## 3. Estructura HTML / Razor base

Copia esta estructura base en el archivo `.razor` del componente en el que quieras aplicar el diseño:

```razor
<div class="dashboard-root">
    <!-- Sidebar Left Panel -->
    <aside class="custom-sidebar">
        <div class="logo-container">
            <a href="/" class="logo-g" title="Logo">G</a>
        </div>
        
        <div class="sidebar-menu">
            <button class="menu-item active" title="Dashboard">
                <svg viewBox="0 0 24 24">
                    <path d="M4 4h4v4H4zm6 0h4v4h-4zm6 0h4v4h-4zM4 10h4v4H4zm6 0h4v4h-4zm6 0h4v4h-4zM4 16h4v4H4zm6 0h4v4h-4zm6 0h4v4h-4z"/>
                </svg>
            </button>
            <button class="menu-item" title="Analytics">
                <svg viewBox="0 0 24 24">
                    <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 17.93c-3.95-.49-7-3.85-7-7.93 0-.62.08-1.21.21-1.79L9 15v1c0 1.1.9 2 2 2v1.93zm6.9-2.54c-.26-.81-1-1.39-1.9-1.39h-1v-3c0-.55-.45-1-1-1H8v-2h2c.55 0 1-.45 1-1V7h2c1.1 0 2-.9 2-2v-.41c2.93 1.19 5 4.06 5 7.41 0 2.08-.8 3.97-2.1 5.39z"/>
                </svg>
            </button>
        </div>
    </aside>

    <!-- Main Content Area -->
    <main class="main-content">
        <!-- Top Navigation -->
        <header class="top-navigation">
            <div class="nav-tabs-wrapper">
                <button class="tab-btn active">Tab 1</button>
                <button class="tab-btn">Tab 2</button>
            </div>
            
            <button class="new-chat-btn">
                <span>+ Botón de Acción</span>
            </button>
        </header>

        <!-- Dashboard Grid Layout -->
        <div class="dashboard-grid">
            <!-- Left Column -->
            <div class="grid-col">
                <!-- Tarjeta Premium -->
                <div class="premium-card recording-card animate-card delay-1">
                    <h3>Contenido de Columna Izquierda</h3>
                </div>
            </div>

            <!-- Right Column -->
            <div class="grid-col">
                <!-- Métricas -->
                <div class="metrics-row">
                    <div class="premium-card metric-card animate-card delay-3">
                        <span>Métrica 1</span>
                    </div>
                    <div class="premium-card metric-card animate-card delay-4">
                        <span>Métrica 2</span>
                    </div>
                </div>
            </div>
        </div>
    </main>
</div>
```
