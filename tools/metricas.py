"""Genera las graficas del analisis estadistico (practica MA de CMMI).

Uso, desde la raiz del repositorio:
    dotnet run -- --bench metrics/bench.csv
    python3 tools/metricas.py

Entradas: metrics/bench.csv, docs/proceso/registro-defectos.csv y el historial de git.
Salidas: metrics/*.png y metrics/resumen.txt
"""
import csv
import statistics as st
import subprocess
from collections import Counter, defaultdict

import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt

NAR, NAR2, CAFE, NEGRO, GRIS = "#FF5900", "#E97132", "#6B3F23", "#1A1A1A", "#BBBBBB"
A2, D3, D4 = 0.577, 0.0, 2.114  # constantes para subgrupos de 5
plt.rcParams.update({"font.family": "DejaVu Sans", "font.size": 10, "axes.spines.top": False,
                     "axes.spines.right": False, "axes.edgecolor": "#555555"})
out = []


def subgrupos(version):
    g = defaultdict(list)
    for r in csv.DictReader(open("metrics/bench.csv")):
        if r["version"] == version:
            g[int(r["subgrupo"])].append(float(r["us_por_cuadro"]))
    return [g[k] for k in sorted(g)]


# 1. Grafico de control X-R de v1.0
grupos = subgrupos("v1.0")
medias = [st.mean(g) for g in grupos]
rangos = [max(g) - min(g) for g in grupos]
xbb, rbar = st.mean(medias), st.mean(rangos)
lsc, lic = xbb + A2 * rbar, max(0.0, xbb - A2 * rbar)
lscr, licr = D4 * rbar, D3 * rbar
fuera = [i + 1 for i, m in enumerate(medias) if m > lsc or m < lic]
fuera_r = [i + 1 for i, r in enumerate(rangos) if r > lscr]
out.append("Grafico X-R v1.0: media general %.3f us, LSC %.3f, LIC %.3f, R medio %.3f, LSC R %.3f" % (xbb, lsc, lic, rbar, lscr))
out.append("Subgrupos fuera de control en X: %s | en R: %s" % (fuera or "ninguno", fuera_r or "ninguno"))

fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(8.6, 5.6), sharex=True, gridspec_kw={"height_ratios": [3, 2]})
x = list(range(1, len(medias) + 1))
ax1.plot(x, medias, marker="o", color=CAFE, lw=1.4, ms=4)
for i in fuera:
    ax1.plot(i, medias[i - 1], "o", color=NAR, ms=9, mfc="none", mew=2)
for y, lab, c, ls in [(lsc, "LSC %.3f" % lsc, NAR, "--"), (xbb, "Media %.3f" % xbb, NEGRO, "-"), (lic, "LIC %.3f" % lic, NAR, "--")]:
    ax1.hlines(y, 0.3, len(x) + 0.6, color=c, ls=ls, lw=1.1)
    ax1.text(len(x) + 0.9, y, lab, va="center", fontsize=8.5, color=c)
ax1.set_ylabel("Media del subgrupo (µs por cuadro)")
ax1.set_title("Gráfico de control X̄: tiempo de actualización por cuadro, v1.0 con 400 plataformas", fontsize=10.5, loc="left")
ax2.plot(x, rangos, marker="s", color=CAFE, lw=1.2, ms=3.5)
for i in fuera_r:
    ax2.plot(i, rangos[i - 1], "s", color=NAR, ms=9, mfc="none", mew=2)
for y, lab, c, ls in [(lscr, "LSC %.3f" % lscr, NAR, "--"), (rbar, "R̄ %.3f" % rbar, NEGRO, "-")]:
    ax2.hlines(y, 0.3, len(x) + 0.6, color=c, ls=ls, lw=1.1)
    ax2.text(len(x) + 0.9, y, lab, va="center", fontsize=8.5, color=c)
ax2.set_ylabel("Rango (µs)")
ax2.set_xlabel("Subgrupo (5 ejecuciones de 44 cuadros cada uno)")
ax2.set_xticks(x)
ax2.set_xlim(0.3, len(x) + 4.2)
fig.tight_layout()
fig.savefig("metrics/grafico-control.png", dpi=200)
plt.close(fig)

# 2. Histograma de tiempos v0 contra v1.0
v0 = [v for g in subgrupos("v0") for v in g]
v1 = [v for g in grupos for v in g]
out.append("v0: media %.3f us, mediana %.3f | v1.0: media %.3f us, mediana %.3f | mejora de la mediana %.1fx"
           % (st.mean(v0), st.median(v0), st.mean(v1), st.median(v1), st.median(v0) / st.median(v1)))
fig, ax = plt.subplots(figsize=(8.6, 3.4))
top = max(max(v0), max(v1))
bins = [i * top / 60 for i in range(0, 62)]
ax.hist(v0, bins=bins, color=GRIS, edgecolor="white", label="v0 (sondeo)")
ax.hist(v1, bins=bins, color=NAR2, edgecolor="white", label="v1.0 (eventos)")
ax.axvline(st.median(v0), color=NEGRO, ls=":", lw=1)
ax.axvline(st.median(v1), color=CAFE, ls=":", lw=1)
ax.set_xlabel("Tiempo de actualización por cuadro (µs)")
ax.set_ylabel("Ejecuciones")
ax.set_title("Distribución de 125 ejecuciones por versión", fontsize=10.5, loc="left")
ax.legend(frameon=False)
fig.tight_layout()
fig.savefig("metrics/histograma-tiempos.png", dpi=200)
plt.close(fig)

# 3. Pareto de defectos por tipo
defs = list(csv.DictReader(open("docs/proceso/registro-defectos.csv")))
por_tipo = Counter(d["tipo"] for d in defs)
nombres = {"Logica": "Lógica", "Configuracion": "Configuración"}
orden = por_tipo.most_common()
etq = [nombres.get(k, k) for k, _ in orden]
val = [v for _, v in orden]
acum, s = [], 0
for v in val:
    s += v
    acum.append(100 * s / sum(val))
fig, ax = plt.subplots(figsize=(8.6, 3.4))
ax.bar(etq, val, color=[NAR if i == 0 else NAR2 for i in range(len(val))], width=0.6)
for i, v in enumerate(val):
    ax.text(i, v + 0.05, str(v), ha="center", fontsize=9)
ax.set_ylabel("Defectos")
ax.set_ylim(0, max(val) + 1)
ax.set_yticks(range(0, max(val) + 2))
ax2 = ax.twinx()
ax2.plot(etq, acum, color=CAFE, marker="o", ms=4)
ax2.set_ylim(0, 105)
ax2.set_ylabel("% acumulado")
ax2.spines["top"].set_visible(False)
ax.set_title("Histograma de Pareto: %d defectos registrados por tipo" % len(defs), fontsize=10.5, loc="left")
fig.tight_layout()
fig.savefig("metrics/pareto-defectos.png", dpi=200)
plt.close(fig)
out.append("Defectos por tipo: " + ", ".join("%s %d" % kv for kv in orden))
abiertos = sum(1 for d in defs if d["estado"] == "Abierto")
out.append("Defectos abiertos: %d de %d" % (abiertos, len(defs)))

# 4. Volumen de cambio por commit (sin package-lock)
log = subprocess.run(["git", "log", "--reverse", "--first-parent", "main", "--numstat", "--format=@%h|%ad|%s", "--date=short"],
                     capture_output=True, text=True).stdout
commits = []
for line in log.splitlines():
    if line.startswith("@"):
        h, d, msg = line[1:].split("|", 2)
        commits.append([h, d, msg, 0])
    elif line.strip() and commits:
        a, r, f = line.split("\t")
        if "package-lock" in f or a == "-":
            continue
        commits[-1][3] += int(a) + int(r)
fig, ax = plt.subplots(figsize=(8.6, 3.6))
ax.barh([c[0] + "  " + c[2][:42] for c in commits][::-1], [c[3] for c in commits][::-1], color=NAR2)
for i, c in enumerate(commits[::-1]):
    ax.text(c[3] + 8, i, str(c[3]), va="center", fontsize=8.5)
ax.set_xlabel("Líneas agregadas más eliminadas (sin package-lock.json)")
ax.set_title("Volumen de cambio por commit en la rama main", fontsize=10.5, loc="left")
ax.tick_params(axis="y", labelsize=8)
fig.tight_layout()
fig.savefig("metrics/volumen-commits.png", dpi=200)
plt.close(fig)
out.append("Commits en main: %d" % len(commits))

open("metrics/resumen.txt", "w").write("\n".join(out) + "\n")
print("\n".join(out))
