sources = files(
	'algo.c',
	'cli.c',
	'consts.c',
	'instance.c',
	'matrice.c',
	'node.c',
	'tour.c',
	'utils.c'
)

if cairo_found
    sources += files('draw.c')
endif

tsp_exe = executable('tsp', sources, 'main.c', dependencies: deps, install: true)
