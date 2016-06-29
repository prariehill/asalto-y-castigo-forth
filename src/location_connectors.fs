\ location_connectors.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606291120

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Herramientas para crear conexiones entre escenarios

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

create exits-table  #exits cells allot
  \ Tabla de traducción de salidas.

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


\ vim:filetype=gforth:fileencoding=utf-8