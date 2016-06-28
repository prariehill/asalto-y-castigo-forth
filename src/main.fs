#! /usr/bin/env gforth

\ main.fs

\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ A text adventure in Spanish,
\ written in Forth with Gforth.

\ Project under development.

\ Version: see file <version.fs>.

\ Last update: 201606282119

\ Copyright (C) 2011..2016 Marcos Cruz (programandala.net)

\ 'Asalto y castigo' is free software; you can redistribute
\ it and/or modify it under the terms of the GNU General
\ Public License as published by the Free Software
\ Foundation; either version 2 of the License, or (at your
\ option) any later version. See:

\ http://gnu.org/licenses/
\ http://gnu.org/licenses/gpl-2.0.html

\ ==============================================================
\ Reconocimiento

\ «Asalto y castigo» está basado en el programa homónimo escrito por
\ Baltasar el Arquero en Sinclair BASIC para ZX Spectrum.

\ Idea, argumento, textos y programa originales:
\
\ Copyright (C) 2009 Baltasar el Arquero
\ http://caad.es/baltasarq/
\ http://baltasarq.info

\ ==============================================================
\ Requisitos

include requirements.fs

\ ==============================================================
\ Listas de palabras

wordlist constant game-wordlist
  \ Palabras del programa (no de la aventura)

: restore-wordlists  ( -- )
  only forth game-wordlist dup >order set-current  ;
  \ Restaura las listas a su orden de búsqueda habitual.

restore-wordlists

wordlist constant player-wordlist
  \ Palabras del jugador.

wordlist constant restore-wordlist
  \ Palabras de restauración de una partida.

\ ==============================================================
\ Meta

include version.fs

: press-key  ( -- ) key drop  ;

\ Indicadores para depuración

false value [debug] immediate
  \ ¿Depuración global?
false value [debug-init] immediate
  \ ¿Depurar la inicialización?
false value [debug-parsing] immediate
  \ ¿Depurar el analizador?
false value [debug-parsing-result] immediate
  \ ¿Mostrar el resultado del analizador?
false value [debug-filing] immediate
  \ ¿Depurar operaciones de ficheros?
false value [debug-do-exits] immediate
  \ ¿Depurar la acción `do-exits`?
false value [debug-catch] immediate
  \ ¿Depurar `catch` y `throw`?
false value [debug-save] immediate
  \ ¿Depurar la grabación de partidas?
true value [debug-info] immediate
  \ ¿Mostrar info sobre el presto de comandos?
false value [debug-pause] immediate
  \ ¿Hacer pausa en puntos de depuración?
false value [debug-map] immediate
  \ ¿Mostrar el número de escenario del juego original?

\ Indicadores para poder elegir alternativas que aún son experimentales

true dup constant [old-method] immediate
      0= constant [new-method] immediate

\ Títulos de sección

: depth-warning  ( -- )
  cr ." Aviso: La pila no está vacía. Contenido: "  ;

: ?.s  ( -- )
  depth if  depth-warning .s cr  press-key  then  ;
  \ Imprime el contenido de la pila si no está vacía.

: section(  ( "text<bracket>" -- )
  cr postpone .(  \ El nombre de sección terminará con: )
  ?.s  ;
  \ Notación para los títulos de sección en el código fuente.
  \ Permite hacer tareas de depuración mientras se compila el programa;
  \ por ejemplo detectar el origen de descuadres en la pila.

: subsection(  ( "text<bracket>" -- )
  cr 2 spaces [char] - emit postpone .(  \ El nombre de subsección terminará con: )
  ?.s  ;
  \ Notación para los títulos de subsección en el código fuente.

\ ==============================================================
\ Vectores

defer protagonist%  ( -- a )  \ Ente protagonista.
defer sword%        ( -- a )  \ Ente espada.
defer stone%        ( -- a )  \ Ente piedra.
defer torch%        ( -- a )  \ Ente antorcha.
defer leader%       ( -- a )  \ Ente líder de los refugiados.
defer location-01%  ( -- a )  \ Primer ente escenario.
defer exits%        ( -- a )  \ Ente salidas.
defer log%          ( -- a )  \ Ente tronco.

defer list-exits  ( -- )
  \ Crea e imprime la lista de salidas.

defer lock-found  ( -- )
  \ Encontrar el candado.

\ ==============================================================
\ Códigos de error

include error_codes.fs
include config_variables.fs
include plot_variables.fs
include display.fs

\ ==============================================================
\ Depuración

: fatal-error  ( f ca len -- )
  rot if  ." Error fatal: " type cr bye  else  2drop  then  ;
  \ Si el indicador _f_ es distinto de cero,
  \ informa de un error _ca len_ y sale del sistema.
  \ XXX TODO -- no usado

: .stack  ( -- )
  [false] [if]  \ XXX OLD
    ." Pila" depth
    if    ." :" .s ." ( " depth . ." )"
    else  ."  vacía."  then
  [else]  \ XXX NEW
    depth if  cr ." Pila: " .s cr  then
  [then]  ;
  \ Imprime el estado de la pila.

: .sb  ( -- )
  ." Espacio para cadenas:" sb# ?  ;
  \ Imprime el estado del almacén circular de cadenas.

: .system-status  ( -- )
  ( .sb ) .stack  ;
  \ Muestra el estado del sistema.

: .debug-message  ( ca len -- )
  dup if  cr type cr  else  2drop  then  ;
  \ Imprime el mensaje del punto de chequeo, si no está vacío.

: debug-pause  ( -- )
  [debug-pause] [if]  depth ?? press-key [then]  ;
  \ Pausa tras mostrar la información de depuración.

: debug  ( ca len -- )
  debug-color .debug-message .system-status debug-pause  ;
  \ Punto de chequeo: imprime un mensaje y muestra el estado del sistema.

\ ==============================================================

include strings.fs
include random_texts.fs
\ include sound.fs  \ XXX TODO -- not used yet
include printing.fs
include data_structure.fs

\ ==============================================================
\ Identificadores de entes

\ Cada ente es identificado mediante una palabra. Los
\ identificadores de entes se crean con la palabra `entity:`.
\ Cuando se ejecutan devuelven la dirección en memoria de la
\ ficha del ente en la base de datos, que después puede ser
\ modificada con un identificador de campo para convertirla en
\ la dirección de memoria de un campo concreto de la ficha.
\
\ Para reconocer mejor los identificadores de entes usamos el
\ sufijo «%» en sus nombres.
\
\ Los entes escenario usan como nombre de identificador el número
\ que tienen en la versión original del programa. Esto hace más
\ fácil la adaptación del código original en BASIC.  Además, para
\ que algunos cálculos tomados del código original funcionen, es
\ preciso que los entes escenario se creen ordenados por ese
\ número.
\
\ El orden en que se definan los restantes identificadores es
\ irrelevante.  Si están agrupados por tipos y en orden
\ alfabético es solo por claridad.

entity: ulfius%
' ulfius% is protagonist%  \ Actualizar el vector que apunta al ente protagonista

\ Entes que son (seudo)personajes:
entity: ambrosio%
entity: (leader%) ' (leader%) is leader%
entity: soldiers%
entity: refugees%
entity: officers%

\ Entes que son objetos:
entity: altar%
entity: arch%
entity: bed%
entity: bridge%
entity: candles%
entity: cave-entrance%
entity: cloak%
entity: cuirasse%
entity: door%
entity: emerald%
entity: fallen-away%
entity: flags%
entity: flint%
entity: grass%
entity: idol%
entity: key%
entity: lake%
entity: lock%
entity: (log%) ' (log%) is log%
entity: piece%
entity: rags%
entity: ravine-wall%
entity: rocks%
entity: snake%
entity: (stone%) ' (stone%) is stone%
entity: (sword%) ' (sword%) is sword%
entity: table%
entity: thread%
entity: (torch%) ' (torch%) is torch%
entity: wall%  \ XXX TODO -- inconcluso
entity: waterfall%

\ Entes escenario (en orden de número):
entity: (location-01%) ' (location-01%) is location-01%
entity: location-02%
entity: location-03%
entity: location-04%
entity: location-05%
entity: location-06%
entity: location-07%
entity: location-08%
entity: location-09%
entity: location-10%
entity: location-11%
entity: location-12%
entity: location-13%
entity: location-14%
entity: location-15%
entity: location-16%
entity: location-17%
entity: location-18%
entity: location-19%
entity: location-20%
entity: location-21%
entity: location-22%
entity: location-23%
entity: location-24%
entity: location-25%
entity: location-26%
entity: location-27%
entity: location-28%
entity: location-29%
entity: location-30%
entity: location-31%
entity: location-32%
entity: location-33%
entity: location-34%
entity: location-35%
entity: location-36%
entity: location-37%
entity: location-38%
entity: location-39%
entity: location-40%
entity: location-41%
entity: location-42%
entity: location-43%
entity: location-44%
entity: location-45%
entity: location-46%
entity: location-47%
entity: location-48%
entity: location-49%
entity: location-50%
entity: location-51%

\ Entes globales:
entity: sky%
entity: floor%
entity: ceiling%
entity: clouds%
entity: cave%  \ XXX TODO -- inconcluso

\ Entes virtuales
\ (necesarios para la ejecución de algunos comandos):
entity: inventory%
entity: (exits%)  ' (exits%) is exits%
entity: north%
entity: south%
entity: east%
entity: west%
entity: up%
entity: down%
entity: out%
entity: in%
entity: enemy%

\ Tras crear los identificadores de entes
\ ya conocemos cuántos entes hay
\ (pues la palabra `entity:` actualiza el contador `#entities`)
\ y por tanto podemos reservar espacio para la base de datos:

\ XXX TODO -- mejorar con el sistema usado en _La pistola de agua_.

#entities /entity * constant /entities
  \ Espacio necesario para guardar todas las fichas.

create ('entities) /entities allot
' ('entities) is 'entities
'entities /entities erase
  \ Crear e inicializar la tabla en el diccionario.

\ ==============================================================
\ Herramientas para crear conexiones entre escenarios

\ XXX TODO -- Mover este código a <data_structure.fs>, para no separar
\ la lista de entes de sus datos.  Pero se necesita usar los
\ identificadores de los entes dirección.  Se podría solucionar con
\ vectores, más adelante.

\ Para crear el mapa hay que hacer dos operaciones con los
\ entes escenario: marcarlos como tales, para poder
\ distinguirlos como escenarios; e indicar a qué otros entes
\ escenario conducen sus salidas.
\
\ La primera operación se hace guardando un valor buleano «cierto»
\ en el campo `~is-location?` del ente.  Por ejemplo:

\   cave% ~is-location? bit-on

\ O bien mediante la palabra creada para ello en la interfaz
\ básica de campos:

\   cave% be-location

\ La segunda operación se hace guardando en los campos de
\ salida del ente los identificadores de los entes a que cada
\ salida conduzca.  No hace falta ocuparse de las salidas
\ impracticables porque ya estarán a cero de forma
\ predeterminada.  Por ejemplo:

\   path% cave% ~south-exit !  \ Hacer que la salida sur de `cave%` conduzca a `path%`
\   cave% path% ~north-exit !  \ Hacer que la salida norte de `path%` conduzca a `cave%`

\ No obstante, para hacer más fácil este segundo paso, hemos
\ creado unas palabras que proporcionan una sintaxis específica,
\ como mostraremos a continuación.

0 [if]  \ XXX TODO -- inconcluso

create opposite-exits
south-exit> ,
north-exit> ,
west-exit> ,
east-exit> ,
down-exit> ,
up-exit> ,
in-exit> ,
out-exit> ,

create opposite-direction-entities
south% ,
north% ,
west% ,
east% ,
down% ,
up% ,
in% ,
out% ,

[then]

\ Necesitamos una tabla que nos permita traducir esto:
\
\ ENTRADA: Un puntero correspondiente a un campo de dirección
\ de salida en la ficha de un ente.
\
\ SALIDA: El identificador del ente dirección al que se
\ refiere esa salida.

create exits-table  \ Tabla de traducción de salidas
#exits cells allot  \ Reservar espacio para tantas celdas como salidas

: >exits-table>  ( u -- a )  first-exit> - exits-table +  ;
  \ Apunta a la dirección de un elemento de la tabla de direcciones.
  \ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)
  \ a = Dirección del ente dirección correspondiente en la tabla

: exits-table!  ( a u -- )  >exits-table> !  ;
  \ Guarda un ente _a_ en una posición de la tabla de salidas.
  \ a = Ente dirección
  \ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)

: exits-table@  ( u -- a )  >exits-table> @  ;
  \ Devuelve un ente dirección a partir de un campo de dirección.
  \ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)
  \ a = Ente dirección

\ Rellenar cada elemento de la tabla con un ente de salida, usando
\ como puntero el campo análogo de la ficha.  Haciéndolo de esta
\ manera no importa el orden en que se rellenen los elementos.

north% north-exit> exits-table!
south% south-exit> exits-table!
east% east-exit> exits-table!
west% west-exit> exits-table!
up% up-exit> exits-table!
down% down-exit> exits-table!
out% out-exit> exits-table!
in% in-exit> exits-table!

0 [if]  \ XXX TODO -- inconcluso
: opposite-exit  ( a1 -- a2 )
  first-exit> - opposite-exits + @  ;
  \ Devuelve la dirección cardinal opuesta a la indicada.

: opposite-exit%  ( a1 -- a2 )
  first-exit> - opposite-direction-entities + @  ;
  \ Devuelve el ente dirección cuya direccién es opuesta a la indicada.
  \ a1 = entidad de dirección
  \ a2 = entidad de dirección, opuesta a a1

[then]

\ A continuación definimos palabras para proporcionar la
\ siguiente sintaxis (primero origen y después destino en la
\ pila, como es convención en Forth):

\   \ Hacer que la salida sur de `cave%` conduzca a `path%`
\   \ pero sin afectar al sentido contrario:
\   cave% path% s-->

\   \ Hacer que la salida norte de `path%` conduzca a `cave%`
\   \ pero sin afectar al sentido contrario:
\   path% cave% n-->

\ O en un solo paso:

\   \ Hacer que la salida sur de `cave%` conduzca a `path%`
\   \ y al contrario: la salida norte de `path%` conducirá a `cave%`:
\   cave% path% s<-->

: -->  ( a1 a2 u -- )  rot + !  ;
  \ Conecta el ente _a1_ (origen) con el ente _a2_ (destino) mediante
  \ la salida de _a1_ indicada por el desplazamiento de campo _u_.

: -->|  ( a1 u -- )  + no-exit swap !  ;
  \ Cierra la salida del ente _a1_ (origen) hacia el ente _a2_
  \ (destino), indicada por el desplazamiento de campo _u_.

\ Conexiones unidireccionales

: n-->  ( a1 a2 -- )  north-exit> -->  ;
  \ Comunica la salida norte del ente _a1_ con el ente _a2_.

: s-->  ( a1 a2 -- )  south-exit> -->  ;
  \ Comunica la salida sur del ente _a1_ con el ente _a2_.

: e-->  ( a1 a2 -- )  east-exit> -->  ;
  \ Comunica la salida este del ente _a1_ con el ente _a2_.

: w-->  ( a1 a2 -- )  west-exit> -->  ;
  \ Comunica la salida oeste del ente _a1_ con el ente _a2_.

: u-->  ( a1 a2 -- )  up-exit> -->  ;
  \ Comunica la salida hacia arriba del ente _a1_ con el ente _a2_.

: d-->  ( a1 a2 -- )  down-exit> -->  ;
  \ Comunica la salida hacia abajo del ente _a1_ con el ente _a2_.

: o-->  ( a1 a2 -- )  out-exit> -->  ;
  \ Comunica la salida hacia fuera del ente _a1_ con el ente _a2_.

: i-->  ( a1 a2 -- )  in-exit> -->  ;
  \ Comunica la salida hacia dentro del ente _a1_ con el ente _a2_.

: n-->|  ( a1 -- )  north-exit> -->|  ;
  \ Desconecta la salida norte del ente _a1_.

: s-->|  ( a1 -- )  south-exit> -->|  ;
  \ Desconecta la salida sur del ente _a1_.

: e-->|  ( a1 -- )  east-exit> -->|  ;
  \ Desconecta la salida este del ente _a1_.

: w-->|  ( a1 -- )  west-exit> -->|  ;
  \ Desconecta la salida oeste del ente _a1_.

: u-->|  ( a1 -- )  up-exit> -->|  ;
  \ Desconecta la salida hacia arriba del ente _a1_.

: d-->|  ( a1 -- )  down-exit> -->|  ;
  \ Desconecta la salida hacia abajo del ente _a1_.

: o-->|  ( a1 -- )  out-exit> -->|  ;
  \ Desconecta la salida hacia fuera del ente _a1_.

: i-->|  ( a1 -- )  in-exit> -->|  ;
  \ Desconecta la salida hacia dentro del ente _a1_.

\ Conexiones bidireccionales

: n<-->  ( a1 a2 -- )  2dup n-->  swap s-->  ;
  \ Comunica la salida norte del ente _a1_ con el ente _a2_ (y al
  \ contrario).

: s<-->  ( a1 a2 -- )  2dup s-->  swap n-->  ;
  \ Comunica la salida sur del ente _ente a1_ con el ente _ente a2_ (y
  \ al contrario).

: e<-->  ( a1 a2 -- )  2dup e-->  swap w-->  ;
  \ Comunica la salida este del ente _ente a1_ con el ente _ente a2_
  \ (y al contrario).

: w<-->  ( a1 a2 -- )  2dup w-->  swap e-->  ;
  \ Comunica la salida oeste del ente _ente a1_ con el ente _ente a2_
  \ (y al contrario).

: u<-->  ( a1 a2 -- )  2dup u-->  swap d-->  ;
  \ Comunica la salida hacia arriba del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: d<-->  ( a1 a2 -- )  2dup d-->  swap u-->  ;
  \ Comunica la salida hacia abajo del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: o<-->  ( a1 a2 -- )  2dup o-->  swap i-->  ;
  \ Comunica la salida hacia fuera del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: i<-->  ( a1 a2 -- )  2dup i-->  swap o-->  ;
  \ Comunica la salida hacia dentro del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: n|<-->|  ( a1 a2 -- )  s-->|  n-->|  ;
  \ Desconecta la salida norte del ente _ente a1_ con el ente _ente
  \ a2_ (y al contrario).

: s|<-->|  ( a1 a2 -- )  n-->|  s-->|  ;
  \ Desconecta la salida sur del ente _ente a1_ con el ente _ente a2_
  \ (y al contrario).

: e|<-->|  ( a1 a2 -- )  w-->|  e-->|  ;
  \ Desconecta la salida este del ente _ente a1_ con el ente _ente a2_
  \ (y al contrario).

: w|<-->|  ( a1 a2 -- )  e-->|  w-->|  ;
  \ Desconecta la salida oeste del ente _ente a1_ con el ente _ente
  \ a2_ (y al contrario).

: u|<-->|  ( a1 a2 -- )  d-->|  u-->|  ;
  \ Desconecta la salida hacia arriba del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: d|<-->|  ( a1 a2 -- )  u-->|  d-->|  ;
  \ Desconecta la salida hacia abajo del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: o|<-->|  ( a1 a2 -- )  i-->|  o-->|  ;
  \ Desconecta la salida hacia fuera del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: i|<-->|  ( a1 a2 -- )  o-->|  i-->|  ;
  \ Desconecta la salida hacia dentro del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

\ Por último, definimos dos palabras para hacer
\ todas las asignaciones de salidas en un solo paso.

: exits!  ( a1 ... a8 a0 -- )
  >r r@ ~out-exit !
     r@ ~in-exit !
     r@ ~down-exit !
     r@ ~up-exit !
     r@ ~west-exit !
     r@ ~east-exit !
     r@ ~south-exit !
     r> ~north-exit !  ;
  \ Asigna todas las salidas _a1 ... a8_ de un ente escenario _a0_.
  \ Los entes de salida _a1 ... a8_ (o cero) están en el orden
  \ habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera.

: init-location  ( a1 ... a8 a0 -- )  dup be-location exits!  ;
  \ Marca un ente _a0_ como escenario y le asigna todas las salidas.
  \ _a1 ... a8_, que sen entes escenario de salida (o cero) en el
  \ orden habitual: norte, sur, este, oeste, arriba, abajo, dentro,
  \ fuera.

\ ==============================================================

include data.fs
include action_errors.fs
include lists.fs
include config.fs
include plot.fs
include actions.fs
include parser.fs
include player_vocabulary.fs
include input.fs
include answers.fs
include the_end.fs
include about.fs
include intro.fs

\ ==============================================================
\ Principal

: init-once  ( -- )  restore-wordlists  init-screen  ;
  \ Preparativos que hay que hacer solo una vez, antes de la primera partida.

: init-parser/game  ( -- )  erase-last-command-elements  ;
  \ Preparativos que hay que hacer en el intérprete
  \ de comandos antes de cada partida.
  \ XXX TODO -- trasladar a su zona

: init-game-for-debugging  ( -- )
  location-01% enter-location
  \ location-08% enter-location  \ Emboscada
  \ location-11% enter-location  \ Lago
  \ location-17% enter-location  \ Antes de la cueva oscura
  \ location-19% enter-location  \ Encuentro con Ambrosio
  \ location-28% enter-location  \ Refugiados
  \ location-47% enter-location  \ casa de Ambrosio
  \ snake% be-here
  \ ambrosio% be-here
  \ key% be-hold
  flint% be-hold
  torch% be-hold
  ;
  \ Condiciones especiales de inicio, para forzar situaciones
  \ concretas de la trama en el arranque y así probar el código.

: init-game  ( -- )
  randomize
  init-parser/game init-entities init-plot
  get-config new-page
  [true] [if]    about cr intro  location-01% enter-location
         [else]  init-game-for-debugging
         [then]  ;
  \ Preparativos que hay que hacer antes de cada partida.

: game  ( -- )  begin  plot accept-command obey  game-over?  until  ;
  \ Bucle de la partida.

: (adventure)  ( -- )  begin  init-game game the-end  enough?  until  ;
' (adventure) is adventure
  \ Bucle del juego.

: run  ( -- )  init-once adventure farewell  ;
  \ Arranque del juego.

forth-wordlist set-current

\ ==============================================================

cr .( Escribe RUN para jugar)  \ XXX TMP

: i0  ( -- )
  \ XXX TMP -- hace toda la inicialización; para depuración.
  init-once init-game
  s" Datos preparados." paragraph  ;

\ i0 cr  \ XXX TMP -- para depuración

\ include ../debug/checks.fs

