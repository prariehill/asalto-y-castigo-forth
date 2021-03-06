\ actions.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201711160145
\ See change log at the end of the file

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/any-question.fs            \ `any?`
require galope/bracket-false.fs           \ `[false]`
require galope/choose.fs                  \ `choose`
require galope/fifty-percent-nullify.fs   \ `50%nullify`
require galope/one-plus-store.fs          \ `1+!`
require galope/question-keep.fs           \ `?keep`
require galope/question-one-plus-store.fs \ `?1+!`
require galope/question-question.fs       \ `??`
require galope/s-curly-bracket.fs         \ `s{`
require galope/shuffle.fs                 \ `shuffle`
require galope/str-prepend-txt.fs         \ `str-prepend-txt`
require galope/stringer.fs                \ Circular string buffer
require galope/svariable.fs               \ `svariable`
require galope/txt-plus.fs                \ `txt+`
require galope/to-yyyymmddhhmmss.fs       \ `>yyyymmddhhss`
require galope/two-choose.fs              \ `2choose`
require galope/ends-question.fs           \ `ends?`

\ Forth Foundation Library
\ http://irdvo.github.io/ffl/

require ffl/str.fs

set-current

require out-str.fs

\ ==============================================================
\ Mensaje de acción completada

require talanto/well-done.fs

' narrate is type-well-done

:noname ( -- ca len )
  s{ s" Hecho." s" Bien." }s  ; is well-done$
  \ Mensaje genérico de que una acción se ha realizado.

: ?well-done ( ca len -- )
  3 random 0= if  2drop well-done  else  well-done-this  then ;
  \ Informa de que una acción se ha realizado, bien con el mensaje _ca
  \ len_ o, al azar (33% de probabilidades), con un mensaje genérico.

\ ==============================================================
\ Comprobación de los requisitos de las acciones

: by-default ( x1 x2 -- x1 | x2 )
  over if  drop  else  nip  then ;
  \ Return _x2_ if _x1_ is zero.

\ ==============================================================
\ Acciones

defer do-attack ( -- )
defer do-break ( -- )
defer do-climb ( -- )
defer do-close ( -- )
defer do-do ( -- )
defer do-drop ( -- )
defer do-examine ( -- )
defer do-exits ( -- )
defer do-frighten ( -- )
defer do-go ( -- )
defer do-go-ahead ( -- )
defer do-go-back ( -- )
defer do-go-down ( -- )
defer do-go-east ( -- )
defer do-go-in ( -- )
defer do-go-north ( -- )
defer do-go|do-break ( -- )
defer do-go-out ( -- )
defer do-go-south ( -- )
defer do-go-up ( -- )
defer do-go-west ( -- )
defer do-hit ( -- )
defer do-introduce-yourself ( -- )
defer do-inventory ( -- )
defer do-kill ( -- )
defer do-look ( -- )
defer do-look-to-direction ( -- )
defer do-look-yourself ( -- )
defer do-make ( -- )
defer do-open ( -- )
defer do-put-on ( -- )
defer do-search ( -- )
defer do-sharpen ( -- )
defer do-speak ( -- )
defer do-swim ( -- )
defer do-take ( -- )
defer do-take|do-eat ( -- ) \ XXX TODO -- cambiar do-eat por ingerir
defer do-take-off ( -- )
defer do-and ( -- )

\ ----------------------------------------------
\ Mirar, examinar y registrar

: (do-look) ( a -- )
  dup describe
  dup is-location? ?? .present familiar++ ;
  \ Mira un ente.

: do-look-by-default ( -- a )
  main-complement my-holder by-default ;
  \ Devuelve qué mirar.  Si falta el complemento principal, usar el
  \ escenario.

:noname ( -- )
  ?no-tool-complement
  do-look-by-default dup ?lookable (do-look)
  ; is do-look
  \  Acción de mirar.

:noname ( -- )
  ?no-tool-complement
  main-complement protagonist~ by-default
  (do-look)
  ; is do-look-yourself
  \  Acción de mirarse.
  \  XXX TODO -- factorizar la operación de elegir un ente
  \  predeterminado `by-default`.

:noname ( -- )
  ?no-tool-complement
  main-complement
  if    main-complement ?direction
        main-complement (do-look)
  else  do-exits  then
  ; is do-look-to-direction
  \  Acción de otear.

:noname ( -- )
  do-look
  ; is do-examine
  \ Acción de examinar.

:noname ( -- )
  do-look
  ; is do-search
  \ Acción de registrar.
  \ XXX TMP

\ ----------------------------------------------
\ Salidas

\ XXX TODO -- Inacabado, no se usa

create do-exits-table-index  #exits cells allot
  \ Tabla para desordenar el listado de salidas.  Esta tabla permite
  \ que las salidas se muestren cada vez en un orden diferente.

variable #free-exits
  \ Contador de las salidas posibles.

: no-exit$ ( -- ca len )
  s" No hay"
  s{ s" salidas" s" salida" s" ninguna salida" }s txt+ ;
  \ Devuelve mensaje usado cuando no hay salidas que listar.

: go-out$ ( -- ca len )
  s{ s" salir" s" seguir" }s ;

: go-out-to& ( ca len -- ca1 len1 )
  go-out$ txt+ s" hacia" txt+ ;

: one-exit-only$ ( -- ca len )
  s{
  s" La única salida" possible1$ txt+ s" es" txt+ s" hacia" 50%nullify txt+
  ^only$ s" hay salida" txt+ possible1$ txt+ s" hacia" txt+
  ^only$ s" es posible" txt+ go-out-to&
  ^only$ s" se puede" txt+ go-out-to&
  }s ;
  \ Devuelve mensaje usado cuando solo hay una salidas que listar.

: possible-exits$ ( -- ca len )
  s" salidas" possible2$ txt+ ;

: several-exits$ ( -- ca len )
  s{
  s" Hay" possible-exits$ txt+ s" hacia" txt+
  s" Las" possible-exits$ txt+ s" son" txt+
  }s ;
  \ Devuelve mensaje usado cuando hay varias salidas que listar.

: .exits ( -- )
  #listed @ case
    0 of  no-exit$  endof
    1 of  one-exit-only$  endof
    several-exits$ rot
  endcase
  out-str str-prepend-txt out-str str-get period+ narrate ;
  \ Imprime las salidas posibles.

: exit-separator$ ( -- ca len )
  #free-exits @ #listed @ list-separator$ ;
  \ Devuelve el separador adecuado a la salida actual.

: exit>list ( u -- )
  [debug-do-exits] [if]  cr ." exit>list" cr .stack  [then]  \ XXX INFORMER
  exit-separator$ out-str str-append-string
  exits-table@ full-name out-str str-append-string
  #listed 1+!
  [debug-do-exits] [if]  cr .stack  [then]  ;  \ XXX INFORMER
  \ Lista una salida.
  \ u = Puntero a un campo de dirección (desplazamiento relativo desde
  \ el inicio de la ficha).

false [if]

  \ XXX OLD -- Primera versión: Las salidas se listan siempre en el
  \ mismo orden en el que están definidas en las fichas.

: free-exits ( a -- u )
  [debug-do-exits] [if]  cr ." free-exits" cr .stack  [then]  \ XXX INFORMER
  0 swap
  ~first-exit /exits bounds do
\   [debug-do-exits] [if]  i i cr . @ .  [then]  \ XXX INFORMER
    i @ 0<> abs +
  cell  +loop
  [debug-do-exits] [if]  cr .stack  [then]  ;  \ XXX INFORMER
  \ Devuelve el número de salidas posibles de un ente.

:noname ( -- )
  out-str str-clear
  #listed off
  my-holder dup free-exits #free-exits !
  last-exit> 1+ first-exit> do
    [debug-do-exits] [??] ~~
    dup i + @
    [debug-do-exits] [??] ~~
    if  i exit>list  then
  cell  +loop  drop
  .exits
  ; is do-exits
  \ Acción de listar las salidas posibles de la localización del protagonista.

[else]

  \ XXX NEW -- Segunda versión: Las salidas se muestran cada vez en
  \ orden aleatorio.

0 value this-location
  \ Ente del que queremos calcular las salidas libres (para
  \ simplificar el manejo de la pila en el bucle).

: free-exits ( a -- a1 ... au u )
  [debug-do-exits] [if]  cr ." free-exits" cr .stack  [then]  \ XXX INFORMER
  to this-location  depth >r
  last-exit> 1+ first-exit> do
    this-location i + @ ?? i
  cell  +loop
  depth r> -
  [debug-do-exits] [if]  cr .stack  [then]  ;  \ XXX INFORMER
  \ Devuelve el número de salidas _u_ posibles de un ente _a_.
  \ _a1 ... au_ son los entes de salida del ente _a_.

: (list-exits) ( -- )
  out-str str-clear
  #listed off
  my-holder free-exits
  dup >r shuffle r>  dup #free-exits !
  0 ?do  exit>list  loop  .exits ;
  \ Crea la lista de salidas y la imprime

' (list-exits) is describe-exits

:noname ( -- )
  ?no-tool-complement
  main-complement ?dup if
    dup my-holder <> swap direction 0= and ?? nonsense.error
  then  describe-exits
  ; is do-exits
  \ Lista las salidas posibles de la localización del protagonista.

[then]

\ ----------------------------------------------
\ Ponerse y quitarse prendas

: >you-take-it$ ( a -- ca len )
  s{ s" Recoges" s" Tomas" s" Coges" }s
  rot full-name txt+ ;
  \ XXX TODO -- configurar español americano

: >you-put-on$ ( a -- ca len )
  s" Te pones" rot full-name txt+ period+ ;

: silently-do-take ( -- )
  show-well-done off  do-take  show-well-done on ;

: (do-put-on) ( a -- )
  dup is-hold?
  if    dup >you-put-on$
  else  silently-do-take
        dup dup >you-take-it$ s" y te" txt+
        rot direct-pronoun txt+ s" pones." txt+
  then ( a ca len ) rot be-worn well-done-this ;
  \ Ponerse una prenda.
  \ XXX TODO -- mejorar el mensaje

:noname ( -- )
  ?no-tool-complement
  ?main-complement
  main-complement ?not-worn-by-me
  main-complement ?wearable
  main-complement (do-put-on)
  ; is do-put-on
  \ Acción de ponerse una prenda.

: >you-take-off-it$ ( a -- ca len )
  s" Te quitas" rot full-name txt+ ;

: you-take-off-main-complement$ ( -- ca len )
  main-complement >you-take-off-it$ ;

: do-take-off-done$ ( -- ca len )
  you-take-off-main-complement$ period+ ;
  \ Devuelve el mensaje que informa de que el protagonista se ha
  \ quitado el complemento principal, una prenda.

: (do-take-off) ( a -- )
  be-not-worn  do-take-off-done$ well-done-this ;
  \ Quitarse una prenda.

:noname ( -- )
  ?no-tool-complement
  ?main-complement
  main-complement ?worn-by-me
  main-complement (do-take-off)
  ; is do-take-off
  \ Acción de quitarse una prenda.

\ ----------------------------------------------
\ Tomar y dejar

\ XXX OLD -- Puede que aún sirva:
\ : cannot-take-the-altar  \ No se puede tomar el altar
\   s" [el altar no se toca]" narrate  \ XXX TMP
\   impossible
\  ;
\ : cannot-take-the-flags  \ No se puede tomar las banderas
\   s" [las banderas no se tocan]" narrate  \ XXX TMP
\   nonsense.error
\  ;
\ : cannot-take-the-idol  \ No se puede tomar el ídolo
\   s" [el ídolo no se toca]" narrate  \ XXX TMP
\   impossible
\  ;
\ : cannot-take-the-door  \ No se puede tomar la puerta
\   s" [la puerta no se toca]" narrate  \ XXX TMP
\   impossible
\  ;
\ : cannot-take-the-fallen-away  \ No se puede tomar el derrumbe
\   s" [el derrumbe no se toca]" narrate  \ XXX TMP
\   nonsense.error
\  ;
\ : cannot-take-the-snake  \ No se puede tomar la serpiente
\   s" [la serpiente no se toca]" narrate  \ XXX TMP
\   dangerous
\  ;
\ : cannot-take-the-lake  \ No se puede tomar el lago
\   s" [el lago no se toca]" narrate  \ XXX TMP
\   nonsense.error
\  ;
\ : cannot-take-the-lock  \ No se puede tomar el candado
\   s" [el candado no se toca]" narrate  \ XXX TMP
\   impossible
\  ;
\ : cannot-take-the-water-fall  \ No se puede tomar la cascada
\   s" [la cascada no se toca]" narrate  \ XXX TMP
\   nonsense.error
\  ;

: cannot-take-the-flags.error ( -- )
  no-reason-for$ s" ofender a" txt+
  talked-to-the-leader?
  if    s" los refugiados."
  else  s{ s" esta gente." s" estas personas." s" nadie." }s
  then  txt+ action-error ;

: (do-take) ( a -- )
  dup be-hold dup familiar++
  >you-take-it$ period+ well-done-this ;
  \ Toma un ente _a_.

: ?do-take ( a -- )
  case
    altar~       of  impossible.error             endof
    door~        of  do-not-worry.error           endof
    fallen-away~ of  nonsense.error               endof
    flags~       of  cannot-take-the-flags.error  endof
    idol~        of  impossible.error             endof
    lake~        of  nonsense.error               endof
    lock~        of  impossible.error             endof
    snake~       of  dangerous.error              endof
    waterfall~   of  nonsense.error               endof
  endcase ;

: cannot-take-a-human ( -- )
  no-reason-for$
  s{ s" molestar" s" incomodar" s" ofender" }s txt+
  main-complement direct-pronoun s+ period+ narrate ;

:noname ( -- )
  ?main-complement
  main-complement ?not-hold
  main-complement ?here
  main-complement is-global? ?? nonsense.error
  main-complement ?do-take
  main-complement is-decoration? ?? do-not-worry.error
  main-complement is-human? if  cannot-take-a-human exit  then
  main-complement (do-take)
  ; is do-take
  \ Toma un ente, si es posible.

: >do-drop-done$ ( a -- ca1 len1 ) { object }
  s{ s" Te desprendes de" s" Dejas" }s
  object full-name txt+ period+ ;

: silently-do-take-off ( -- )
  show-well-done off  do-take-off  show-well-done on ;

: (do-drop) ( a -- ) { object }
  object is-worn?
  if    silently-do-take-off
        you-take-off-main-complement$ s" y" txt+
        s{ s" te desprendes de" object personal-pronoun txt+
        object direct-pronoun s" dejas" txt+ }s txt+ period+
  else  object >do-drop-done$
  then ( ca len ) object be-here  well-done-this ;
  \ Deja un ente _a_ que está en inventario.

:noname ( -- )
  \ Acción de dejar.
  ?main-complement
  main-complement ?hold
  main-complement (do-drop)
  ; is do-drop

:noname ( -- )
  \ Acción de desambiguación.
  \ XXX TODO
  do-take
  ; is do-take|do-eat

\ ----------------------------------------------
\ Cerrar y abrir

: first-close-the-door ( -- )
  s" cierras" s" primero" rnd2swap txt+ xcapitalized
  door~ full-name txt+ period+ narrate
  door~ be-closed ;
  \ Informa de que la puerta está abierta
  \ y hay que cerrarla antes de poder cerrar el candado.

: .the-key-fits ( -- )
  s" La llave gira fácilmente dentro del candado."
  narrate ;
  \ XXX TODO -- nuevo texto, quitar «fácilmente»

: close-the-lock ( -- )
  lock~ ?accessible
  key~ ?this-tool
  lock~ ?open
  key~ ?hold
  door~ is-open? ?? first-close-the-door
  lock~ be-closed  .the-key-fits ;
  \ Cerrar el candado, si es posible.
  \ XXX TODO gestionar la herramienta

: .the-door-closes ( -- )
  s" La puerta"
  s{ s" rechina" s" emite un chirrido" }s txt+
  s{ s" mientras la cierras" s" al cerrarse" }s txt+
  period+ narrate ;
  \ Muestra el mensaje de cierre de la puerta.

: (close-the-door) ( -- )
  door~ be-closed .the-door-closes
  location-47~ location-48~ w|<-->|
  location-47~ location-48~ o|<-->| ;
  \ Cerrar la puerta.

: close-and-lock-the-door ( -- )
  door~ ?open  key~ ?hold
  (close-the-door) close-the-lock ;
  \ Cerrar la puerta, si está abierta, y el candado.

: just-close-the-door ( -- )
  door~ ?open (close-the-door) ;
  \ Cerrar la puerta, sin candarla, si está abierta.

: close-the-door ( -- )
  door~ ?accessible
  tool-complement ?dup
  if    key~ ?this-tool
        close-and-lock-the-door
  else  just-close-the-door  then ;
  \ Cerrar la puerta, si es posible.

: close-it ( a -- )
  case
    door~ of  close-the-door  endof
    lock~ of  close-the-lock  endof
    nonsense.error
  endcase ;
  \ Cerrar un ente, si es posible.

:noname ( -- )
  ?main-complement
  main-complement close-it
  ; is do-close
  \ Acción de cerrar.

: the-door-is-locked ( -- )
  lock~ ^full-name s" bloquea la puerta." txt+
  narrate
  lock-found ;
  \ Informa de que la puerta está cerrada por el candado.
  \ XXX TODO -- añadir variantes

: unlock-the-door ( -- )
  the-door-is-locked
  key~ ?needed
  lock~ dup be-open
  ^pronoun s" abres con" txt+ key~ full-name txt+ period+ narrate ;
  \ Abrir la puerta candada, si es posible.
  \ XXX TODO -- falta mensaje adecuado sobre la llave que gira

: open-the-lock ( -- )
  lock~ ?accessible
  key~ ?this-tool
  lock~ ?closed
  key~ ?needed
  lock~ be-open  well-done ;
  \ Abrir el candado, si es posible.
  \ XXX TODO -- abrirlo aunque no se indique herramienta, si tenemos
  \ la llave.

: the-plants$ ( -- ca len )
  s" las hiedras" s" las hierbas" both ;
  \ Devuelve las plantas que la puerta rompe al abrirse.
  \ XXX TODO -- hacerlas visibles

: the-door-breaks-the-plants-0$ ( -- ca len )
  s{ s" mientras" s" al tiempo que" }s
  the-plants$ txt+ s" se rompen en su trazado" txt+ ;
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta
  \ (primera variante).

: the-door-breaks-the-plants-1$ ( -- ca len )
  s" rompiendo" the-plants$ txt+ s" a su paso" txt+ ;
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta
  \ (segunda variante).

: the-door-breaks-the-plants$ ( -- ca len )
  ['] the-door-breaks-the-plants-0$
  ['] the-door-breaks-the-plants-1$ 2 choose execute ;
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta.

: the-door-sounds$ ( -- ca len )
  s{ s" rechinando" s" con un chirrido" }s ;

: ambrosio-byes ( -- )
  s" Ambrosio, alegre, se despide de ti:" narrate
  s{
  s{ s" Tengo" s" Ten" }s s" por seguro" txt+
  s{ s" Estoy" s" Estate" }s s" seguro de" txt+
  s{ s" Tengo" s" Ten" }s s" la" txt+ s{ s" seguridad" s" certeza" }s txt+ s" de" txt+
  s" No" s{ s" me cabe" s" te quepa" }s txt+ s" duda de" txt+
  s" No" s{ s" dudo" s" dudes" }s txt+
  }s s" que" txt+
  s{
  s" nos volveremos a" s{ s" encontrar" s" ver" }s txt+ again$ 50%nullify txt+
  s" volveremos a" s{ s" encontrarnos" s" vernos" }s txt+ again$ 50%nullify txt+
  s" nos" s{ s" encontraremos" s" veremos" }s txt+ again$ txt+
  s" nuestros caminos" s{ s" volverán a cruzarse" s" se cruzarán" }s txt+ again$ txt+
  }s txt+ period+ speak
  ^and|but$ s" , antes de que puedas" s+
  s{ s" decir algo" s" corresponderle" s" responderle" s" despedirte" s" de él" 50%nullify txt+ }s txt+
  s" , te das cuenta de que" s+ s" Ambrosio" 50%nullify txt+
  s" , misteriosamente," 50%nullify s+
  s{ s" ha desaparecido" s" se ha marchado" s" se ha ido" s" ya no está" }s txt+
  period+ narrate ;
  \ Ambrosio se despide cuando se abre la puerta por primera vez.

: the-door-opens-first-time$ ( -- ca len )
  s" La puerta" s{ s" cede" s" se abre" }s txt+
  s{ s" despacio" s" poco a poco" s" lentamente" }s txt+
  s" y no sin" txt+
  s{ s" dificultad" s" ofrecer resistencia" }s txt+ comma+
  the-door-sounds$ comma+ txt+
  the-door-breaks-the-plants$ txt+ period+ ;
  \ Devuelve el mensaje de apertura de la puerta
  \ la primera vez.

: the-door-opens-once-more$ ( -- ca len )
  s" La puerta se abre" the-door-sounds$ txt+ period+ ;
  \ Devuelve el mensaje de apertura de la puerta
  \ la segunda y siguientes veces.

: .the-door-opens ( -- )
  door~ times-open
  if    the-door-opens-once-more$ narrate
  else  the-door-opens-first-time$ narrate ambrosio-byes  then ;
  \ Muestra el mensaje de apertura de la puerta.

: (open-the-door) ( -- )
  key~ ?this-tool  \ XXX TODO ¿por qué aquí?
  lock~ is-closed? ?? unlock-the-door
  location-47~ location-48~ w<-->
  location-47~ location-48~ o<-->
  .the-door-opens
  door~ dup be-open times-open++
  grass~ be-here ;
  \ Abrir la puerta, que está cerrada.

: open-the-door ( -- )
  door~ ?accessible
  door~ ?open
  ?no-tool-complement
  (open-the-door) ;
  \ Abrir la puerta, si es posible.

: open-it ( a -- )
  dup familiar++
  case
    door~ of  open-the-door  endof
    lock~ of  open-the-lock  endof
    nonsense.error
  endcase ;
  \ Abrir un ente, si es posible.

:noname ( -- )
  ?main-complement
  main-complement open-it
  ; is do-open
  \ Acción de abrir.

\ ----------------------------------------------
\ Agredir

: the-snake-runs-away$ ( -- ca len )
  s" la serpiente" txt+
  s{ s" huye" s" se aleja" s" se esconde"
     s" se da a la fuga" s" se quita de enmedio"
     s" se aparta" s" escapa" }s txt+
  s{ null$ s" asustada" s" atemorizada" }s txt+ period+ ;
  \ Texto de que la serpiente huye.

: the-snake-runs-away ( ca len -- )
  the-snake-runs-away$ txt+ narrate snake~ vanish ;

: attack-the-snake ( -- )
  sword~ ?needed
  s{ s" Sorprendida por" s" Ante" }s
  s" los amenazadores tajos," the-snake-runs-away ;
  \ Atacar la serpiente.
  \ XXX TODO -- gestionar la herramienta

: (do-attack) ( a -- )
  case
    snake~    of  attack-the-snake  endof
    ambrosio~ of  no-reason.error   endof
    leader~   of  no-reason.error   endof
    soldiers~ of  no-reason.error   endof
    do-not-worry.error
  endcase ;
  \ Atacar un ser vivo.

:noname ( -- )
  ?main-complement
  main-complement ?living
  main-complement ?accessible
  main-complement (do-attack)
  ; is do-attack
  \ Acción de atacar.

: (do-frighten) ( a -- )
  case
    snake~    of  attack-the-snake  endof
    ambrosio~ of  no-reason.error   endof
    leader~   of  no-reason.error   endof
    soldiers~ of  no-reason.error   endof
    do-not-worry.error
  endcase ;
  \ Asustar un ser vivo.
  \ XXX TODO diferenciar de atacar

:noname ( -- )
  ?main-complement
  main-complement ?beast
  main-complement ?accessible
  main-complement (do-frighten)
  ; is do-frighten
  \ Acción de asustar.

: kill-the-snake ( -- )
  sword~ ?needed
  s{ s" Sorprendida por" s" Ante" }s
  s" los amenazadores tajos," the-snake-runs-away ;
  \ Matar la serpiente.
  \ XXX TODO -- gestionar herramienta
  \ XXX TODO -- diferenciar de atacar

: (do-kill) ( a -- )
  case
    snake~    of  kill-the-snake   endof
    ambrosio~ of  no-reason.error  endof
    leader~   of  no-reason.error  endof
    soldiers~ of  no-reason.error  endof
    do-not-worry.error
  endcase ;
  \ Matar un ser vivo.
  \ XXX TODO diferenciar de atacar

:noname ( -- )
  ?main-complement
  main-complement ?living
  main-complement ?accessible
  main-complement (do-kill)
  ; is do-kill
  \ Acción de matar.
  \ XXX TODO diferenciar de atacar

: cloak-piece ( a -- )
  2 random if  be-here  else  be-hold  then ;
  \ Hace aparecer un resto de la capa rota de forma aleatoria:
  \ en el escenario o en el inventario.
  \ XXX TODO -- mejorar con mensajes, ejemplo:
  \ s" Un hilo se ha desprendido al cortar la capa con la espada."

: cloak-pieces ( -- )
  rags~ cloak-piece  thread~ cloak-piece  piece~ cloak-piece ;
  \ Hace aparecer los restos de la capa rota de forma aleatoria:
  \ en el escenario o en el inventario.

: take-sword-if-needed ( -- )
  sword~ is-not-hold? if  sword~ (do-take)  then ;

: shatter-the-cloak-with-the-sword ( -- )
  take-sword-if-needed
  using$ sword~ full-name txt+ comma+
  s" rasgas" txt+ cloak~ full-name txt+ period+ narrate
  cloak-pieces  cloak~ vanish ;

: objects-that-could-cut ( -- a#1..a#n n )
  arch~ cuirasse~ emerald~ flint~ idol~ key~
  lock~ rocks~ stone~ table~ torch~  11 ;
  \ Entities that could be used to cut something, not including the
  \ sword.

: could-cut? ( a -- f ) objects-that-could-cut any? ;
  \ Could entity _a_ be used to cut something (without considering the
  \ sword)?

: shatter-the-cloak-with-a-useless-tool ( a -- )
  dup ?accessible useless-tool.error ;

: shatter-the-cloak-with-a-tool ( a -- )
  dup is-living-being? ?? nonsense.error
  dup could-cut? ?? shatter-the-cloak-with-a-useless-tool
  sword~ ?this-tool
  sword~ ?accessible
  shatter-the-cloak-with-the-sword ;

: shatter-the-cloak-without-a-explicit-tool ( -- )
  sword~ is-not-accessible? ?? not-by-hand.error
  shatter-the-cloak-with-the-sword ;

: try-to-shatter-the-cloak ( -- )
  tool-complement ?dup
  if    shatter-the-cloak-with-a-tool
  else  shatter-the-cloak-without-a-explicit-tool  then ;
  \ Intenta romper la capa.

: (do-break) ( a -- )
  case
    snake~ of  kill-the-snake            endof  \ XXX TMP
    cloak~ of  try-to-shatter-the-cloak  endof
    do-not-worry.error
  endcase ;
  \ Romper un ente.

:noname ( -- )
  ?main-complement
  main-complement ?accessible
  \ main-complement ?breakable  \ XXX TODO
  main-complement (do-break)
  ; is do-break
  \ Acción de romper.

: lit-the-torch ( -- )
  s" Poderosas chispas salen del choque entre espada y pedernal,"
  s" encendiendo la antorcha." txt+ narrate
  torch~ be-lighted ;
  \ XXX TODO -- variar el texto

: hit-the-flint ( -- )
  flint~ ?accessible
  sword~ be-hold
  using$ sword~ full-name txt+ comma+
  s" golpeas" txt+ flint~ full-name txt+ period+ narrate
  lit-the-torch ;

: (do-hit) ( a -- )
  case
    snake~ of  kill-the-snake            endof
    cloak~ of  try-to-shatter-the-cloak  endof
    flint~ of  hit-the-flint             endof
    do-not-worry.error
  endcase ;
  \ Golpear un ente.

:noname ( -- )
  ?main-complement
  main-complement ?accessible
  main-complement (do-hit)
  \ s" golpear"  main-complement+is-nonsense \ XXX TMP
  ; is do-hit
  \ Acción de golpear.

: log-already-sharpened$ ( -- ca len )
  s" Ya" s{ s" lo afilaste antes"
            s" está afilado de antes"
            s" tiene una buena punta"
            s" quedó antes bien afilado"
         }s txt+ ;
  \ Devuelve una variante de «Ya está afilado».

: no-need-to-do-it-again$ ( -- ca len )
  s{
  s" no es necesario"
  s" no hace" s" ninguna" 50%nullify txt+ s" falta" txt+
  s" no es menester"
  s" no serviría de nada"
  s" no serviría de mucho"
  s" serviría de poco"
  s" sería inútil"
  s" sería en balde"
  s" sería un esfuerzo" s{ s" inútil" s" baldío" }s txt+
  s" sería un esfuerzo baldío"
  }s s{
  s" hacerlo"
  s" volver a hacerlo"
  s" repetirlo"
  }s txt+ again$ txt+ ;
  \ Devuelve una variante de «no hace falta hacerlo otra vez».

: ^no-need-to-do-it-again$ ( -- ca len )
  no-need-to-do-it-again$ xcapitalized ;
  \ Devuelve una variante de «No hace falta hacerlo otra vez».

: log-already-sharpened-0$ ( -- ca len )
  log-already-sharpened$ xcapitalized period+
  ^no-need-to-do-it-again$ period+ txt+ ;
  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 0).

: log-already-sharpened-1$ ( -- ca len )
  ^no-need-to-do-it-again$ period+ txt+
  log-already-sharpened$ xcapitalized period+ txt+ ;
  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 1).

: log-already-sharpened ( -- )
  ['] log-already-sharpened-0$
  ['] log-already-sharpened-1$ 2 choose execute  narrate ;
  \ Informa de que el tronco ya estaba afilado.

: sharpen-the-log ( -- )
  hacked-the-log? @
  if    log-already-sharpened
  else  hacked-the-log? on  well-done  then ;
  \ Afila el tronco.
  \ XXX TODO -- distinguir herramientas

: sharpen-the-sword ( -- ) ;
  \ Afila la espada.
  \ XXX TODO -- inconcluso

: (do-sharpen) ( a -- )
  case
    sword~ of  sharpen-the-sword  endof
    log~ of  sharpen-the-log  endof
  endcase ;
  \ Afila un ente que puede ser afilado.

:noname ( -- )
  \ Acción de afilar.
  ?main-complement
  main-complement ?accessible
  main-complement can-be-sharpened?
  if    main-complement (do-sharpen)
  else  nonsense.error
  then  ; is do-sharpen

\ ----------------------------------------------
\ Movimiento

: do-go-if-possible ( a -- )
  dup ?direction  dup exit-from-here ?dup
  if    nip enter-location
  else  impossible-move-to-it.error
  then ;
  \ Comprueba si el movimiento hacia un supuesto ente de dirección _a_
  \ es posible y si es así lo efectúa.

: simply-do-go ( -- ) s" Ir sin rumbo...?" narrate ;
  \ Ir sin dirección específica.
  \ XXX TODO -- inconcluso

:noname ( -- )
  [debug] [if]  s" Al entrar en `do-go`" debug  [then]  \ XXX INFORMER
  ?no-tool-complement
  main-complement ?dup
  if  do-go-if-possible  else  simply-do-go  then
  [debug] [if]  s" Al salir de `do-go`" debug  [then]  \ XXX INFORMER
  ; is do-go
  \ Acción de ir.

:noname ( -- )
  ?no-tool-complement
  north~ ?this-main-complement
  north~ do-go-if-possible
  ; is do-go-north
  \ Acción de ir al norte.

:noname ( -- )
  ?no-tool-complement
  south~ ?this-main-complement
  south~ do-go-if-possible
  ; is do-go-south
  \ Acción de ir al sur.

:noname ( -- )
  ?no-tool-complement
  east~ ?this-main-complement
  east~ do-go-if-possible
  ; is do-go-east
  \ Acción de ir al este.

:noname ( -- )
  ?no-tool-complement
  west~ ?this-main-complement
  west~ do-go-if-possible
  ; is do-go-west
  \ Acción de ir al oeste.

:noname ( -- )
  ?no-tool-complement
  up~ ?this-main-complement
  up~ do-go-if-possible
  ; is do-go-up
  \ Acción de ir hacia arriba.

:noname ( -- )
  ?no-tool-complement
  down~ ?this-main-complement
  down~ do-go-if-possible
  ; is do-go-down
  \ Acción de ir hacia abajo.

:noname ( -- )
  ?no-tool-complement
  out~ ?this-main-complement
  out~ do-go-if-possible
  ; is do-go-out
  \ Acción de ir hacia fuera.

: enter-the-cave-entrance ( -- )
  cave-entrance~ ?accessible
  if    in~ do-go-if-possible
  else  cave-entrance~ cannot-be-seen.error
  then ;
  \ XXX FIXME -- dónde se actualiza `what` aquí?

:noname ( -- )
  ?no-tool-complement
  main-complement cave-entrance~ =
  if  enter-the-cave-entrance exit  then
  in~ ?this-main-complement
  in~ do-go-if-possible
  ; is do-go-in
  \ Acción de ir hacia dentro.
  \ XXX TODO añadir lago, agua y otros

:noname ( -- )
  ?no-tool-complement
  ?no-main-complement
  s" [voy hacia atrás, pero es broma]" narrate \ XXX TMP
  ; is do-go-back
  \ Acción de ir hacia atrás.
  \ XXX TODO

:noname ( -- )
  ?no-tool-complement
  ?no-main-complement
  s" [voy hacia delante, pero es broma]" narrate \ XXX TMP
  ; is do-go-ahead
  \ Acción de ir hacia delante.
  \ XXX TODO

\ ----------------------------------------------
\ Partir (desambiguación)

:noname ( -- )
  main-complement ?dup
  if ( a )
    is-direction? if  do-go  else  do-break  then
  else
    tool-complement if do-break  else  simply-do-go  then
  then
  ; is do-go|do-break
  \ Acción de partir (desambiguar: romper o marchar).

\ ----------------------------------------------
\ Nadar

: in-a-different-place$ ( -- ca len )
  s" en un" txt+ place$
  s{ s" desconocido" s" nuevo" s" diferente" }s txt+
  s" en otra parte"
  s" en otro lugar"
  3 2choose ;
  \ Devuelve una variante de «en un lugar diferente».

: you-emerge$ ( -- ca len )
  s{ s" Consigues" s" Logras" }s
  s{ s" emerger," s" salir a la superficie," }s txt+
  though$ txt+ in-a-different-place$ txt+
  s" de la" txt+ cave$ txt+ s" ..." s+ ;
  \ Devuelve mensaje sobre la salida a la superficie.

: swiming$ ( -- ca len )
  s" Buceas" s{ s" pensando en" s" deseando"
  s" con la esperanza de" s" con la intención de" }s txt+
  s{ s" avanzar," s" huir," s" escapar,"  s" salir," }s txt+
  s" aunque" txt+ s{ s" perdido." s" desorientado." }s txt+ ;
  \ Devuelve mensaje sobre el buceo.

: drop-the-cuirasse$ ( f -- ca len )
  s{ s" te desprendes de ella" s" te deshaces de ella"
  s" la dejas caer" s" la sueltas" }s
  rot if
    s{ s" Rápidamente" s" Sin dilación"
    s" Sin dudarlo" s{ null$ s" un momento" s" un instante" }s txt+
    }s 2swap txt+
  then  period+ ;
  \ Devuelve mensaje sobre deshacerse de la coraza dentro del agua.
  \ El indicador _f_ es cierto si el resultado debe ser el inicio de
  \ una frase.

: you-leave-the-cuirasse$ ( -- ca len )
  cuirasse~ is-worn-by-me?
  if  s{ s" Como puedes," s" No sin dificultad," }s
      s{ s" logras quitártela" s" te la quitas" }s txt+
      s" y" txt+ false drop-the-cuirasse$ txt+
  else  true drop-the-cuirasse$  then ;
  \ Devuelve mensaje sobre quitarse y soltar la coraza dentro del agua.

: (you-sink-0)$ ( -- ca len )
  s{ s" Caes" s" Te hundes"
  s{ s" Empiezas" s" Comienzas" }s s{ s" a hundirte" s" a caer" }s txt+
  }s s" sin remedio" 50%nullify txt+ s" hacia" txt+
  s{ s" el fondo" s" las profundidades" }s txt+
  s{ s" por el" s" debido al" s" arrastrado por" s" a causa del" }s txt+
  s" peso de tu coraza" txt+ ;
  \ Devuelve la primera versión del mensaje sobre hundirse con la coraza.

: (you-sink-1)$ ( -- ca len )
  s" El peso de tu coraza"
  s{ s" te arrastra" s" tira de ti" }s txt+
  s{ null$ s" sin remedio" s" con fuerza" }s txt+
  s{
  s" hacia el fondo"
  s" hacia las profundidades"
  s" hacia abajo"
  }s txt+ ;
  \ Devuelve la segunda versión del mensaje sobre hundirse con la coraza.

: you-sink$ ( -- ca len )
  ['] (you-sink-0)$
  ['] (you-sink-1)$ 2 choose execute period+ ;
  \ Devuelve mensaje sobre hundirse con la coraza.

: you-swim-with-cuirasse$ ( -- ca len )
  you-sink$ you-leave-the-cuirasse$ txt+ ;
  \  Devuelve mensaje inicial sobre nadar con coraza.

: you-swim$ ( -- ca len )
  cuirasse~ is-hold?
  if  you-swim-with-cuirasse$  cuirasse~ vanish
  else  null$
  then  swiming$ txt+ ;
  \  Devuelve mensaje sobre nadar.

:noname ( -- )
  location-11~ am-i-there?
  if    clear-screen-for-location
        you-swim$ narrate narration-break
        you-emerge$ narrate narration-break
        location-12~ enter-location  the-battle-ends
  else  s" nadar" now-or-here-or-null$ txt+
        that-is-nonsense.error
  then  ; is do-swim
  \ Acción de nadar.
  \ XXX FIXME -- añadir el lago

\ ----------------------------------------------
\ Escalar

: you-try-climbing-the-fallen-away ( -- )
  s{ s" Aunque" s" A pesar de que" }s
  s{  s" parece no haber salida"
      s" el obstáculo parece insuperable"
      s" la situación parece desesperada"
      s" el regreso parece inevitable"
      s" continuar parece imposible"
  }s txt+ comma+
  s{ s" optas por" s" decides" s" tomas la decisión de" }s txt+
  s{ s" explorar" s" examinar" }s txt+ s" el" txt+
  s{ s" derrumbe" s" muro de" rocks$ txt+ }s txt+
  s{  s" en compañía de" s" junto con"
      s" ayudado por" s" acompañado por"
  }s txt+
  s{ s" algunos" s" varios" }s txt+ s" de tus" txt+
  s{ s" oficiales" soldiers$ }s txt+ s" , con la" s+
  s{ s" vana" s" pobre" s" débil" }s txt+ s" esperanza" txt+
  s" de" txt+
  s{ s" encontrar" s" hallar" s" descubrir" }s txt+
  s{ s" la" s" alguna" }s txt+ way$ txt+ s" de" txt+
  s{  s" escalarlo" s" vencerlo" s" atravesarlo"
      s" superarlo" s" pasarlo"
      s{ s" pasar" s" cruzar" }s s" al otro lado" txt+
  }s txt+  period+ narrate narration-break ;
  \ Imprime la primera parte del mensaje
  \ previo al primer intento de escalar el derrumbe.

: you-can-not-climb-the-fallen-away ( -- )
  ^but$
  s{  s{ s" pronto" s" enseguida" }s s" has de" txt+
      s" no tardas mucho en"
  }s txt+
  s{  s" rendirte ante" s" aceptar" s" resignarte ante" }s txt+
  s{
    s{ s" los hechos" s" la realidad" s" la situación" s" tu suerte" }s
    s{  s" la voluntad"
        s{ s" el" s" este" }s s" giro" txt+
        s{ s" el" s" este" }s s" capricho" txt+
        s{ s" la" s" esta" }s s" mudanza" txt+
    }s s" de" txt+ s" la diosa" 50%nullify txt+ s" Fortuna" txt+
  }s txt+ s" ..." s+ narrate narration-break ;
  \ Imprime la segunda parte del mensaje
  \ previo al primer intento de escalar el derrumbe.

: do-climb-the-fallen-away-first ( -- )
  you-try-climbing-the-fallen-away
  you-can-not-climb-the-fallen-away ;
  \ Imprime el mensaje
  \ previo al primer intento de escalar el derrumbe.

: climbing-the-fallen-away$ ( -- ca len )
  s{ s" pasar" s" escalar" s" subir por" }s
  s{
    s" el derrumbe"
    s{ s" el muro" s" la pared" s" el montón" }s s" de" txt+ rocks$ txt+
    s" las" rocks$ txt+
  }s txt+ ;
  \ Devuelve el mensaje de error de que es imposible escalar el
  \ derrumbe.

: do-climb-the-fallen-away ( -- )
  \ Escalar el derrumbe.
  climbed-the-fallen-away? @ 0= ?? do-climb-the-fallen-away-first
  climbed-the-fallen-away? on
  climbing-the-fallen-away$ that-is-impossible.error ;

: do-climb-this-here-if-possible ( a -- ) ;
  \ Escalar el ente indicado, que está presente, si es posible.
  \ XXX TODO -- inconcluso

: do-climb-if-possible ( a -- )
  dup is-here?
  if    drop s" [escalar eso]" narrate
  else  drop s" [no está aquí eso para escalarlo]" narrate
  then ;
  \ Escalar el ente indicado si es posible.
  \ XXX TODO -- inconcluso

: nothing-to-climb ( -- )
  s" [No hay nada que escalar]" narrate ;
  \ XXX TODO -- inconcluso

: do-climb-something ( -- )
  location-09~ am-i-there?  \ ¿Ante el derrumbe?
  if  do-climb-the-fallen-away exit
  then
  location-08~ am-i-there?  \ ¿En el desfiladero?
  if  s" [Escalar en el desfiladero]" narrate exit
  then
  my-holder is-indoor-location?
  if  s" [Escalar en un interior]" narrate exit
  then
  nothing-to-climb ;
  \ Escalar algo no especificado.
  \ XXX TODO -- inconcluso

:noname ( -- )
  main-complement ?dup
  if  do-climb-if-possible  else  do-climb-something  then
  ; is do-climb
  \ Acción de escalar.
  \ XXX TODO -- inconcluso

\ ----------------------------------------------
\ Inventario

: anything-with-you$ ( -- ca len )
  s" nada" with-you$ ?dup if   2 random ?? 2swap txt+
                          else  drop  then ;
  \ Devuelve una variante de «nada contigo».

: you-are-carrying-nothing$ ( -- ca len )
  s" No" you-carry$ anything-with-you$ period+ txt+ txt+ ;
  \ Devuelve mensaje para sustituir a un inventario vacío.

: ^you-are-carrying$ ( -- ca len )
  ^you-carry$ with-you$ txt+ ;
  \ Devuelve mensaje para encabezar la lista de inventario,
  \ con la primera letra mayúscula.

: you-are-carrying$ ( -- ca len ) you-carry$ with-you$ txt+ ;
  \ Devuelve mensaje para encabezar la lista de inventario.

: you-are-carrying-only$ ( -- ca len )
  2 random if    ^you-are-carrying$ only-or-null$ txt+
           else  ^only-or-null$ you-are-carrying$ txt+  then ;
  \ Devuelve mensaje para encabezar una lista de inventario de un solo elemento.

:noname ( -- )
  protagonist~ content-list
  #listed @ case
    0 of  you-are-carrying-nothing$ 2swap txt+ endof
    1 of  you-are-carrying-only$ 2swap txt+ endof
    >r ^you-are-carrying$ 2swap txt+ r>
  endcase  narrate
  ; is do-inventory
  \ Acción de hacer inventario.

' do-inventory is describe-inventory

\ ----------------------------------------------
\ Hacer

:noname ( -- )
  main-complement
  if  nonsense.error  else  do-not-worry.error  then
  ; is do-make
  \ Acción de hacer (fabricar).

:noname ( -- )
  main-complement inventory~ =
  if  do-inventory  else  do-make  then
  ; is do-do
  \ Acción de hacer (genérica).

\ ----------------------------------------------
\ Hablar y presentarse

\ ..............................
\ Conversaciones con el líder de los refugiados

: a-man-takes-the-stone ( -- )
  s{  s" Un hombre" s" Un refugiado"
      s" Uno de los" s{ s" hombres" s" refugiados" }s txt+
  }s s{ s" se adelanta,"
      s" sale de entre"
        s{  s" sus compañeros" s" los otros"
            s" la multitud" s" la gente"
        }s txt+ comma+
  }s 50%nullify txt+
  s{  s" se te acerca," s" se acerca a ti,"
      s" se te aproxima," s" se aproxima a ti,"
  }s 50%nullify txt+
  s" te" txt+ s{ s" arrebata" s" quita" }s txt+
  s" la piedra" txt+ s" de las manos" 50%nullify txt+ s" y" txt+
  s{
    s" se la lleva"
    s{ s" se marcha" s" se va" s" desaparece" }s s" con ella" 50%nullify txt+
  }s txt+ period+ narrate
  location-18~ stone~ be-there ;
  \ Un hombre te quita la piedra.

: gets-angry$ ( -- ca len )
  s" se" s{ s" irrita" s" enfada" s" enoja" s" enfurece" }s txt+ ;
  \ Devuelve una variante de «se enfada».

: the-leader-gets-angry$ ( -- ca len )
  s{ s" Al verte" s" Viéndote" s" Tras verte" }s
  s{ s" llegar" s" aparecer" s" venir" }s txt+
  again$ stone-forbidden? @ ?keep txt+
  s" con la piedra," txt+
  s" el" txt+ old-man$ txt+ gets-angry$ txt+ ;
  \ Devuelve una variante de «El líder se enfada».
  \ XXX OLD -- yo no se usa.

: the-leader-gets-angry ( -- )
  the-leader-gets-angry$ period+ narrate ;
  \ Mensaje de que el líder se enfada.
  \ XXX OLD -- ya no se usa.

: warned-once$ ( -- ca len )
  s{
  s" antes"
  s" en la ocasión anterior"
  s" en la otra ocasión"
  s" en una ocasión"
  s" la otra vez"
  s" la vez anterior"
  s" una vez"
  }s ;

: warned-twice$ ( -- ca len )
  s{
  s" antes"
  s" dos veces"
  s" en dos ocasiones"
  s" en las otras ocasiones"
  s" en mas de una ocasión"
  s" en un par de ocasiones"
  s" las otras veces"
  s" más de una vez"
  s" un par de veces"
  }s ;

: warned-several-times$ ( -- ca len )
  s{
  s" en las otras ocasiones"
  s" en más de dos ocasiones"
  s" en más de un par de ocasiones"
  s" en otras ocasiones"
  s" en varias ocasiones"
  s" las otras veces"
  s" más de dos veces"
  s" más de un par de veces"
  s" otras veces"
  s" varias veces"
  }s ;

: warned-many-times$ ( -- ca len )
  s{
  s" demasiadas veces"
  s" en demasiadas ocasiones"
  s" en incontables ocasiones"
  s" en innumerables ocasiones"
  s" en las otras ocasiones"
  s" en muchas ocasiones"
  s" en varias ocasiones"
  s" incontables veces"
  s" innumerables veces"
  s" las otras veces"
  s" muchas veces"
  s" otras veces"
  s" varias veces"
  }s ;

: times-warned ( u -- ca1 len1 )
  { times }  \ Variable local
  true case
    times 0 = of  null$  endof
    times 1 = of  warned-once$  endof
    times 2 = of  warned-twice$  endof
    times 6 < of  warned-several-times$  endof
    warned-many-times$ rot
  endcase ;

: already-warned$ ( -- ca len )
  s" ya" 50%nullify
  s{
    s" fuisteis" s{ s" avisado" s" advertido" }s txt+ s" de ello" 50%nullify txt+
    s" se os" s{ s" avisó" s" advirtió" }s txt+ s" de ello" 50%nullify txt+
    s" os lo" s{ s" hicimos saber" s" advertimos" }s txt+
    s" os lo" s{ s" hice saber" s" advertí" }s txt+
    s" se os" s{ s" hizo saber" s" dejó claro" }s txt+
  }s txt+ ;
  \ Mensaje de que el líder ya te advirtió sobre un objeto.
  \ XXX TODO -- elaborar más

: already-warned ( u -- ca1 len1 )
  times-warned already-warned$ rnd2swap txt+ period+ xcapitalized ;
  \ Mensaje de que el líder ya te advirtió sobre un objeto,
  \ con indicación al azar del número de veces.

: you-can-not-take-the-stone$ ( -- ca len )
  s{ s" No" s" En modo alguno" s" De ninguna" way$ txt+ s" De ningún modo" }s
  s" podemos" txt+
  s{
    s{ s" permitiros" s" consentiros" }s
      s{ s" huir" s" escapar" s" marchar" s" pasar" }s txt+ s" con" txt+
    s{ s" permitir" s" consentir" s" aceptar" }s s" que" txt+
      s{  s{ s" huyáis" s" escapéis" s" marchéis" s" paséis" }s s" con" txt+
          s" os vayáis con"
          s" os" 50%nullify s" marchéis con" txt+
          s" os llevéis"
          s" nos" 50%nullify s" robéis" txt+
          s" os" s{ s" apropiés" s" apoderéis" s" adueñéis" }s txt+ s" de" txt+
      }s txt+
  }s txt+ s" la" s" piedra del druida"
  2dup stone~ fs-name!
  txt+ txt+ period+ ;
  \ Devuelve el mensaje de que no te puedes llevar la piedra.
  \ También cambia el nombre de la piedra.

: gesture-about-the-stone$ ( -- ca len )
  s" y" 50%nullify s{ s" entonces" s" a continuación" s" seguidamente" }s txt+ xcapitalized
  s" hace un" txt+
  s" pequeño" 50%nullify txt+ s" gesto" txt+ s" con la mano," 50%nullify txt+
  s" casi imperceptible" 50%nullify txt+
  s" ..." s+ ;
  \ Mensaje de que el líder hace un gesto sobre la piedra.

: the-stone-must-be-in-its-place$ ( -- ca len )
  s" La piedra" s{ s" ha de" s" debe" s" tiene que" }s txt+
  s{ s" ser devuelta" s" devolverse" to-go-back$ }s txt+
  s{
    s" a su lugar" s" de encierro" 50%nullify txt+
    s" al lugar al que pertenece"
    s" al lugar del que nunca debió" s{ s" salir" s" ser sacada" s" ser arrancada" }s txt+
    s" al lugar que nunca debió" s{ s" abandonar" s" haber abandonado" }s txt+
  }s txt+ ;
  \ El líder dice que la piedra debe ser devuelta.

: the-leader-warns-about-the-stone ( -- )
  stone-forbidden? @ already-warned
  you-can-not-take-the-stone$
  the-stone-must-be-in-its-place$ rnd2swap txt+ txt+
  period+ speak ;
  \ El líder habla acerca de la piedra.

: the-leader-points-to-the-north$ ( -- ca len )
  leader~ ^full-name
  s{ s" alza" s" extiende" s" levanta" }s txt+
  s{ s" su" s" el" }s txt+ s" brazo" txt+
  s{ s" indicando" s" en dirección" s" señalando" }s txt+
  toward-the(m)$ txt+ s" norte." txt+ ;
  \ El líder se enfada y apunta al norte.
  \ XXX TODO -- crear ente "brazo" aquí, o activarlo como sinómino del anciano

: the-leader-points-to-the-north ( -- )
  the-leader-points-to-the-north$ narrate ;
  \ El líder se enfada y apunta al norte.

: nobody-passes-with-arms$ ( -- ca len )
  s{ s" Nadie" s" Ningún hombre" }s
  s{ s" con" s" llevando" s" portando" s" portador de"
  s" que porte" s" que lleve" }s txt+
  s{ s" armas" s" un arma" s" una espada" }s txt+
  with-him$ txt+ s{ s" debe" s" puede" s" podrá" }s txt+
  s" pasar." txt+ ;
  \ El líder dice que nadie pasa con armas.

: the-leader-warns-about-the-sword ( -- )
  the-leader-points-to-the-north
  sword-forbidden? @ already-warned
  nobody-passes-with-arms$ txt+ speak ;
  \ El líder habla acerca de la espada.

: the-leader-points-to-the-east ( -- )
  s" El" old-man$ txt+ comma+
  s{ s" confiado" s" calmado" s" sereno" s" tranquilo" }s txt+ comma+
  s{ s" indica" s" señala" }s txt+
  s{ toward-the(m)$ s" en dirección al" }s txt+ s" este y" txt+
  s{  s" te" 50%nullify s" dice" txt+
      s" pronuncia las siguientes palabras"
  }s txt+ colon+ narrate ;
  \ El líder apunta al este.

: something-had-been-forbidden? ( -- f )
  sword-forbidden? @ stone-forbidden? @ or ;
  \ Se le prohibió alguna vez al protagonista pasar con algo prohibido?

: go-in-peace ( -- )
  s{ s" Ya que" s" Puesto que" s" Dado que" s" Pues" }s
  something-had-been-forbidden? if
    s{ s" esta vez" s" ahora" s" en esta ocasión" s" por fin" s" finalmente" }s txt+
  then
  s{ s" vienes" s" llegas" s" has venido" s" has llegado" }s txt+
  s" en paz, puedes" txt+
  s{ s" ir" s" marchar" s" continuar" s" tu camino" 50%nullify txt+ }s txt+
  s" en paz." txt+ speak ;
  \ El líder dice que puedes ir en paz.

: the-refugees-let-you-go ( -- )
  s" todos" 50%nullify s" los refugiados" txt+ xcapitalized
  s" se apartan y" txt+ s" te" 50%nullify txt+
  s{  s" permiten" s{ s" el paso" s" pasar" }s txt+
      s" dejan" s" libre" s" el" s{ s" paso" s" camino" }s txt+ rnd2swap txt+
  }s txt+ toward-the(m)$ txt+ s" este." txt+ narrate ;
  \ Los refugiados te dejan pasar.

: the-leader-lets-you-go ( -- )
  location-28~ location-29~ e-->
  the-leader-points-to-the-east
  go-in-peace the-refugees-let-you-go ;
  \ El jefe deja marchar al protagonista.

: talked-to-the-leader ( -- )
  leader~ conversations++
  recent-talks-to-the-leader ?1+! ;
  \ Aumentar el contador de conversaciones con el jefe de los refugiados.

: we-are-refugees$ ( -- ca len )
  s" todos" 50%nullify s" nosotros" 50%nullify rnd2swap txt+
  s" somos refugiados de" txt+ xcapitalized
  s{ s" la gran" s" esta terrible" }s txt+ s" guerra." txt+
  s" refugio" location-28~ ms-name! ;
  \ Mensaje «Somos refugiados».

: we-are-refugees ( -- )
  we-are-refugees$ we-want-peace$ txt+ speak narration-break ;
  \ Somos refugiados.

: the-leader-trusts ( -- )
  s" El" old-man$ txt+ s" asiente" txt+
  s{ s" confiado" s" con confianza" }s txt+ s" y," txt+
  s" con un suave gesto" txt+ s" de su mano" 50%nullify txt+ comma+
  s" te interrumpe para" txt+
  s{  s" explicar" s{ s" te" s" se" null$ }s s+
      s" presentarse" s" contarte" s" decir" s" te" 50%nullify s+
  }s txt+ colon+ narrate ;
  \ El líder te corta, confiado.

: untrust$ ( -- ca len )
  s{ s" desconfianza" s" nerviosismo" s" impaciencia" }s ;

: the-leader-does-not-trust ( -- )
  s" El" old-man$ txt+ s" asiente" txt+ s" con la cabeza" 50%nullify txt+ comma+
  s{  s" desconfiado" s" nervioso" s" impaciente"
      s" mostrando" s" claros" 50%nullify txt+ s" signos de" txt+ untrust$ txt+
      s{ s" dando" s" con" }s s" claras" 50%nullify txt+ s" muestras de" txt+ untrust$ txt+
  }s txt+ comma+ s" y te interrumpe:" txt+ narrate ;
  \ El líder te corta, desconfiado.

: the-leader-introduces-himself ( -- )
  do-you-hold-something-forbidden?
  if    the-leader-does-not-trust
  else  the-leader-trusts
  then  we-are-refugees ;
  \ El líder se presenta.

: first-conversation-with-the-leader ( -- )
  my-name-is$ s" Ulfius y..." txt+ speak talked-to-the-leader
  the-leader-introduces-himself ;
  \ XXX TODO -- elaborar mejor el texto

: the-leader-forbids-the-stone ( -- )
  the-leader-warns-about-the-stone
  stone-forbidden? ?1+!  \ Recordarlo
  gesture-about-the-stone$ narrate narration-break
  a-man-takes-the-stone ;
  \ El jefe te avisa de que no puedes pasar con la piedra y te la quita.

: the-leader-forbids-the-sword ( -- )
  the-leader-warns-about-the-sword  sword-forbidden? ?1+! ;
  \ El jefe te avisa de que no puedes pasar con la espada.
  \ El programa recuerda este hecho incrementando un contador.

: the-leader-checks-what-you-carry ( -- )
  true case
    stone~ is-accessible? of  the-leader-forbids-the-stone  endof
    sword~ is-accessible? of  the-leader-forbids-the-sword  endof
    the-leader-lets-you-go
  endcase ;
  \ El jefe controla lo que llevas.
  \ XXX TODO -- mejorar para que se pueda pasar si dejamos el objeto
  \ en el suelo o se lo damos

: insisted-once$ ( -- ca len )
  s{ s" antes" s" una vez" }s ;
  \ XXX TODO -- inconcluso

: insisted-twice$ ( -- ca len )
  s{ s" antes" s" dos veces" s" un par de veces" }s ;
  \ XXX TODO -- inconcluso

: insisted-several-times$ ( -- ca len )
  s{ s" las otras" s" más de dos" s" más de un par de" s" varias" }s
  s" veces" txt+ ;
  \ XXX TODO -- inconcluso

: insisted-many-times$ ( -- ca len )
  s{  s" demasiadas" s" incontables" s" innumerables"
      s" las otras" s" muchas" s" varias"
  }s  s" veces" txt+ ;
  \ XXX TODO -- inconcluso

: times-insisted ( u -- ca1 len1 )
  { times }
  true case
    times 0 = of  null$  endof  \ XXX OLD -- innecesario
    times 1 = of  insisted-once$  endof
    times 2 = of  insisted-twice$  endof
    times 6 < of  insisted-several-times$  endof
    insisted-many-times$ rot
  endcase ;
  \ XXX TODO -- inconcluso

: please-don't-insist$ ( -- ca len )
  s{ s" os ruego que" s" os lo ruego," s" he de rogaros que" }s
  s" no insistáis" txt+ ;
  \ Mensaje de que por favor no insistas.

: don't-insist$ ( -- ca len )
  s" ya" 50%nullify
  s{
    s" habéis sido" s{ s" avisado" s" advertido" }s txt+
    s" os lo he" s" mos" 50%nullify s+ s{ s" hecho saber" s" advertido" s" dejado claro" }s txt+
    s" se os ha" s{ s" hecho saber" s" advertido" s" dejado claro" }s txt+
  }s txt+ ;
  \ XXX TODO -- inconcluso

: don't-insist ( -- )
  times-insisted don't-insist$ rnd2swap txt+ period+ xcapitalized ;
  \ XXX TODO -- inconcluso

: the-leader-ignores-you ( -- ) ;
  \ El líder te ignora.
  \ XXX TODO

: (talk-to-the-leader) ( -- )
  leader~ no-conversations?
  ?? first-conversation-with-the-leader
  the-leader-checks-what-you-carry ;
  \ Hablar con el jefe.

: talk-to-the-leader ( -- )
  recent-talks-to-the-leader @
  if    the-leader-ignores-you
  else  (talk-to-the-leader)  then ;
  \ Hablar con el jefe, si se puede.

\ ..............................
\ Conversaciones con Ambrosio

: talked-to-ambrosio ( -- ) ambrosio~ conversations++ ;
  \ Aumentar el contador de conversaciones con Ambrosio.

: be-ambrosio's-name ( ca len -- )
  ambrosio~ ms-name!
  ambrosio~ have-no-article
  ambrosio~ have-personal-name ;
  \ Le pone a ambrosio su nombre de pila _ca len_.

: ambrosio-introduces-himself ( -- )
  s" Hola, Ulfius."
  my-name-is$ txt+ s" Ambrosio" 2dup be-ambrosio's-name
  period+ txt+ speak ;

: you-cry ( -- )
  s" Por" s" primera" s" vez" rnd2swap txt+ txt+ s" en" txt+
  s{ s" mucho" s" largo" }s txt+ s" tiempo, te sientas y" txt+
  s" le" 50%nullify txt+ s{ s" cuentas" s" narras" s" relatas" }s txt+
  s" a alguien todo lo que ha" txt+
  s{ s" sucedido" s" pasado" s" ocurrido" }s txt+ period+
  s" Y, tras tanto" txt+ s" pesar" 50%nullify txt+ s{ s" acontecido" s" vivido" }s txt+
  s" , lloras" s+ s{ s" desconsoladamente" s" sin consuelo" }s txt+
  period+ narrate ;

: ambrosio-proposes-a-deal ( -- )
  s" Ambrosio te propone un" s{ s" trato" s" acuerdo" }s txt+ comma+
  s{  the-that(m)$ s" aceptas" txt+
      s" con el" that(m)$ txt+ s{ s" consientes" s" estás de acuerdo" }s txt+
      the-that(m)$ s" te parece justo" txt+
  }s txt+ colon+
  s" por ayudarlo a salir de" txt+ s{ s" la" s" esta" }s txt+ s" cueva," txt+
  s{ s" objetos" s" útiles" }s txt+ comma+
  s{ s" vitales" s" imprescindibles" s" necesarios" }s txt+
  s" para" txt+ s" el éxito de" 50%nullify txt+
  s{ s" la" s" tal" s" dicha" }s txt+ s{ s" misión" s" empresa" }s txt+
  s" , te son entregados." s+ narrate
  torch~ be-hold  flint~ be-hold ;

: ambrosio-let's-go ( -- )
  s{  s" Bien"
      s{ s" Venga" s" Vamos" }s s" pues" 50%nullify txt+
  }s comma+ s" Ambrosio," txt+
  s{  s{ s" iniciemos" s" emprendamos" }s s{ s" la marcha" s" el camino" }s txt+
      s" pongámonos en" s{ s" marcha" s" camino" }s txt+
  }s txt+ period+  speak
  location-46~ ambrosio~ be-there
  s" Te" s{ s" giras" s" das la vuelta" }s txt+ s" para" txt+
  s{  s{ s" comprobar" s" ver" }s s" si" txt+
      s{ s" cerciorarte" s" asegurarte" }s s" de que" txt+
  }s txt+ s" Ambrosio te sigue," txt+ but$ txt+ s" ..." s+
  s{  s" ha desaparecido"
      s" se ha esfumado"
      s" no hay" s" ni" 50%nullify txt+ s" rastro de él" txt+
      s" ya" 50%nullify s" no está" txt+
      s" ya" 50%nullify s" no hay nadie" txt+
      s" ya" 50%nullify s" no ves a nadie" txt+
      s" es como si se lo hubiera tragado la tierra"
  }s txt+ period+ narrate ;

: ambrosio-is-gone ( -- )
  s{  suddenly|then$ s" piensas" rnd2swap txt+ s" en el" txt+
      suddenly|then$ s" caes en la cuenta" rnd2swap txt+ s" del" txt+
  }s xcapitalized s" hecho" s" curioso" rnd2swap txt+ txt+ s" de que" txt+
  s{  s{ s" supiera" s" conociera" }s s{ s" cómo te llamas" s" tu nombre" }s txt+
      s" te llamara por tu nombre"
  }s txt+ s" ..." s+ narrate ;

: (conversation-0-with-ambrosio) ( -- )
  s" Hola, buen hombre." speak
  ambrosio-introduces-himself scene-break
  you-cry scene-break
  ambrosio-proposes-a-deal narration-break
  ambrosio-let's-go narration-break
  ambrosio-is-gone
  talked-to-ambrosio ;
  \ Primera conversación con Ambrosio.

: conversation-0-with-ambrosio ( -- )
  location-19~ am-i-there?
  ?? (conversation-0-with-ambrosio) ;
  \ Primera conversación con Ambrosio, si se dan las condiciones.

: i-am-stuck-in-the-cave$ ( -- ca len )
  s{  s" por desgracia" s" desgraciadamente" s" desafortunadamente"
      s" tristemente" s" lamentablemente"
  }s 50%nullify s{ s" estoy" s" me encuentro" s" me hallo" }s txt+ xcapitalized
  s{ s" atrapado" s" encerrado" }s txt+
  s" en" txt+ s{ s" la" s" esta" }s txt+ s" cueva" txt+
  s{ s" debido a" s" por causa de" s" por influjo de" }s txt+
  s{ s" una" s" cierta" }s txt+ s" magia de" txt+
  s{ s" maligno" s" maléfico" s" malvado" s" terrible" }s txt+
  s" poder." txt+ ;

: you-must-follow-your-way$ ( -- ca len )
  s{ s" En cuanto" s" Por lo que respecta" }s txt+
  s" al camino, vos" txt+
  s{ s" habéis de" s" debéis" s" habréis de" }s txt+
  s{ s" recorrer" s" seguir" s" hacer " }s txt+ s" el vuestro," txt+
  s{ s" ver" s" mirar" s" contemplar" }s s" lo" 50%nullify s+
  s" todo con vuestros" txt+ s" propios" 50%nullify txt+ s" ojos." txt+ ;

: ambrosio-explains ( -- )
  s" Ambrosio"
  s{  s" parece meditar un instante"
      s" asiente ligeramente con la cabeza"
  }s txt+ s" y" txt+
  s{  s" te" s{ s" dice" s" explica" }s txt+
      s" se explica"
  }s txt+
  colon+ narrate
  i-am-stuck-in-the-cave$ you-must-follow-your-way$ txt+ speak ;

: i-can-not-understand-it$ ( -- ca len )
  s" no"
  s{  s" lo" 50%nullify s{ s" entiendo" s" comprendo" }s txt+
      s{ s" alcanzo" s" acierto" }s s" a" txt+
         s{ s" entender" s" comprender" }s txt+ s" lo" 50%nullify s+
  }s txt+ ;

: you-shake-your-head ( -- )
  s{ s" Sacudes" s" Mueves" s" Haces un gesto con" }s s" la cabeza" txt+
  s{  s{ s" poniendo"  s" dejando" }s
        s{ s" clara" s" de manifiesto" s" patente" s" manifiesta" }s txt+
      s{ s" manifestando" s" delatando" s" mostrando" }s s" claramente" 50%nullify txt+
  }s s" tu" txt+
  s{ s" sorpresa" s" perplejidad" s" resignación" s" incredulidad" }s txt+ 50%nullify txt+
  colon+ narrate ;

: you-don't-understand ( -- )
  s{  i-can-not-understand-it$ s" , la verdad" 50%nullify s+
      s{ s" la verdad" s" lo cierto" }s s" es que" txt+
        i-can-not-understand-it$ txt+
      s{ s" en verdad" s" realmente" s" verdaderamente" }s
        i-can-not-understand-it$ txt+
  }s xcapitalized speak ;

: you-already-had-the-key$ ( -- ca len )
  s{
    s" La llave, Ambrosio, estaba ya en vuestro poder."
    s" Vos, Ambrosio, estabais ya en posesión de la llave."
    s" Vos, Ambrosio, ya teníais la llave en vuestro poder."
  }s ;
  \ XXX TODO -- ampliar y variar

: you-know-other-way$ ( -- ca len )
  s" Y" s{ s" por lo demás" s" por otra parte" }s 50%nullify txt+
  s{ s" es" s" parece" }s txt+
  s{ s" obvio" s" evidente" s" claro" s" indudable" }s txt+
  s" que" txt+ s{ s" conocéis" s" sabéis" s" no desconocéis" }s txt+
  s{ s" un" s" algún" s" otro" }s txt+ s" camino" txt+
  s{  s" más" s{ s" corto" s" directo" s" fácil" s" llevadero" }s txt+
      s" menos" s{ s" largo" s" luengo" s" difícil" s" pesado" }s txt+
  }s txt+ period+ ;

: you-reproach-ambrosio ( -- )
  you-already-had-the-key$ you-know-other-way$ txt+ speak ;
  \ Reprochas a Ambrosio acerca de la llave y el camino.

: (conversation-1-with-ambrosio) ( -- )
  you-reproach-ambrosio ambrosio-explains
  you-shake-your-head you-don't-understand
  talked-to-ambrosio ;
  \ Segunda conversación con Ambrosio.

: conversation-1-with-ambrosio ( -- )
  location-46~ am-i-there?
  ambrosio-follows? 0=  and
  ?? (conversation-1-with-ambrosio) ;
  \ Segunda conversación con Ambrosio, si se dan las condiciones.

: ambrosio-gives-you-the-key ( -- )
  s{ s" Por favor," s" Os lo ruego," }s
  s" Ulfius," txt+
  s" cumplid vuestra" s{ s" promesa." s" palabra." }s txt+
  s" Tomad" this|the(f)$ txt+ s" llave" txt+
  s{ null$ s" en vuestra mano" s" en vuestras manos" s" con vos" }s txt+
  s" y abrid" txt+ s" la puerta de" 50%nullify txt+ this|the(f)$ txt+ s" cueva." txt+
  speak
  key~ be-hold ;

: (conversation-2-with-ambrosio) ( -- )
  ambrosio-gives-you-the-key
  ambrosio-follows? on  talked-to-ambrosio ;
  \ Tercera conversación con Ambrosio.
  \ XXX TODO -- hacer que la llave se pueda transportar

: conversation-2-with-ambrosio ( -- )
  location-45~ 1- location-47~ 1+ my-holder within
  ?? (conversation-2-with-ambrosio) ;
  \ Tercera conversación con Ambrosio, si se dan las condiciones.
  \ XXX TODO -- simplificar la condición

false [if]

  \ XXX OLD -- Primera versión, con una estructura `case`

: (talk-to-ambrosio) ( -- )
  ambrosio~ conversations case
    0 of  conversation-0-with-ambrosio  endof
    1 of  conversation-1-with-ambrosio  endof
    2 of  conversation-2-with-ambrosio  endof
  endcase ;
  \ Hablar con Ambrosio.
  \ XXX TODO -- Implementar qué hacer cuando ya no hay más
  \ conversaciones.

[else]

  \ XXX NEW -- Segunda versión, más «estilo Forth», con una tabla de
  \ ejecución.

create conversations-with-ambrosio
  ' (conversation-0-with-ambrosio) ,
  ' (conversation-1-with-ambrosio) ,
  ' (conversation-2-with-ambrosio) ,
  ' noop ,
  \ XXX TODO -- Implementar qué hacer cuando ya no hay más
  \ conversaciones.

: (talk-to-ambrosio) ( -- )
  ambrosio~ conversations cells conversations-with-ambrosio + perform ;
  \ Hablar con Ambrosio.
  \ XXX FIXME comprobar el máximo de conversaciones

[then]

: talk-to-ambrosio ( -- )
  ambrosio~ ?here
  (talk-to-ambrosio) ;
  \ Hablar con Ambrosio, si se puede.
  \ XXX TODO -- esto debería comprobarse en `do-speak` o
  \ `do-speak-if-possible`.

\ ..............................
\ Conversaciones sin éxito

: talk-to-something ( a -- )
  2 random
  if    drop nonsense.error
  else  full-name s" hablar con" 2swap txt+
        that-is-nonsense.error
  then ;
  \ Hablar con un ente que no es un personaje.
  \ XXX TODO

: talk-to-yourself$ ( -- ca len )
  s{  s" hablar" s{ s" solo" s" con uno mismo" }s txt+
      s" hablarse" s{ s" a sí" s" a uno" }s txt+ s" mismo" 50%nullify txt+
  }s ;
  \ Devuelve una variante de «hablar solo».

: talk-to-yourself ( -- )
  talk-to-yourself$  that-is-nonsense.error ;
  \ Hablar solo.

\ ..............................
\ Acciones

: do-speak-if-possible ( a -- )
  [debug] [if]  s" En `do-speak-if-possible`" debug  [then]  \ XXX INFORMER
  case
    leader~ of  talk-to-the-leader  endof
    ambrosio~ of  talk-to-ambrosio  endof
    dup talk-to-something
  endcase ;
  \ Hablar con un ente si es posible.

: (do-speak) ( a | 0 -- )
  ?dup if  do-speak-if-possible  else  talk-to-yourself  then ;
  \ Hablar con alguien o solo.

: (you-speak-to) ( a -- )
  dup familiar++
  s" Hablas con" rot full-name txt+ colon+ narrate ;

: you-speak-to ( a | 0 -- ) ?dup ?? (you-speak-to) ;

:noname ( -- )
  [debug] [??] debug  \ XXX INFORMER
  ?no-main-complement
  secondary-complement whom by-default
  dup you-speak-to (do-speak)
  ; is do-speak
  \ Acción de hablar.
  \ XXX TODO -- revisar

:noname ( -- )
  main-complement unknown-whom by-default (do-speak)
  ; is do-introduce-yourself
  \ Acción de presentarse a alguien.

\ ----------------------------------------------
\ Acción especial «y»

:noname ( -- )
  action if  execute-action init-parser  then
  reuse-previous-action on  ; is do-and

\ ----------------------------------------------
\ Guardar el juego

\ Para guardar el estado de la partida usaremos una solución muy
\ sencilla: ficheros de texto que reproduzcan el código Forth
\ necesario para restaurarlas. Esto permitirá restaurar una partida
\ con solo interpretar ese fichero como cualquier otro código fuente.

false [if]

  \ XXX TODO -- pendiente
  \ XXX OLD

: yyyymmddhhmmss$ ( -- ca len )
  time&date >yyyymmddhhmmss ;
  \ Devuelve la fecha y hora actuales como una cadena en formato
  \ «aaaammddhhmmss».

: file-name$ ( -- ca len )
  \ Devuelve el nombre con que se grabará el juego.
  s" ayc-" yyyymmddhhmmss$ s+ s" .exe" windows? and s+  ;  \ Añadir
  sufijo si estamos en Windows

defer reenter

svariable filename

: (save-the-game) ( -- )
\ false to spf-init?  \ Desactivar la inicialización del sistema
\ true to console?  \ Activar el modo de consola (no está claro en el manual)
\ false to gui?  \ Desactivar el modo gráfico (no está claro en el manual)
  ['] reenter to <main>  \ Actualizar la palabra que se ejecutará al arrancar
\ file-name$ save  new-page
  file-name$ filename place filename count save ;
  \ Graba el juego.
  \ XXX TODO -- no está decidido el sistema que se usará para salvar
  \ las partidas
  \ XXX FIXME -- 2011-12-01 No funciona bien. Muestra mensajes de gcc con
  \ parámetros sacados de textos del programa!

: save-the-game
  ?no-main-complement
  action ? key drop  \ XXX INFORMER
  (save-the-game) ;
  \ Acción de salvar el juego.

[then]

svariable game-file-name
  \ Nombre del fichero en que se graba la partida.

variable game-file-id
  \ Identificador del fichero en que se graba la partida.

: game-file-name$ ( -- ca len ) game-file-name count ;
  \ Devuelve el nombre del fichero en que se graba la partida.

: close-game-file ( -- )
  game-file-id @ close-file abort" Close file error." ;
  \ Cierra el fichero en que se grabó la partida.
  \ XXX TODO -- mensaje de error definitivo

: create-game-file ( ca len -- )
  r/w create-file abort" Create file error."
  game-file-id ! ;
  \ Crea un fichero de nambre _ca len_ para grabar una partida
  \ (sobreescribiendo otro que tuviera el mismo nombre).
  \ XXX TODO -- mensaje de error definitivo

wordlist constant restore-wordlist
  \ Palabras de restauración de una partida.

: read-game-file ( ca len -- )
  restore-wordlist 1 set-order  included  restore-wordlists ;
  \ Lee el fichero de configuración de nombre _ca len_.
  \ XXX TODO -- comprobar la existencia del fichero y atrapar errores
  \ al leerlo

: >file/ ( ca len -- )
  game-file-id @ write-line abort" Write file error" ;
  \ Escribe una línea en el fichero de la partida.
  \ XXX TODO -- mensaje de error definitivo

: cr>file ( -- ) s" " >file/ ;
  \ Escribe un final de línea en el fichero de la partida.

: >file ( ca len -- )
  space+
  game-file-id @ write-file abort" Write file error" ;
  \ Escribe una cadena en el fichero de la partida.
  \ XXX TODO -- mensaje de error definitivo

restore-wordlist set-current

' \ alias \
immediate

' true alias true

' false alias false

' s" alias s"

: load-entity ( x0 ... xn u -- )
  #>entity >r
  \ cr .s  \ XXX INFORMER
  r@ ~direction !
  #>entity r@ ~in-exit !
  #>entity r@ ~out-exit !
  #>entity r@ ~down-exit !
  #>entity r@ ~up-exit !
  #>entity r@ ~west-exit !
  #>entity r@ ~east-exit !
  #>entity r@ ~south-exit !
  #>entity r@ ~north-exit !
  r@ ~familiar !
  r@ ~visits !
  #>entity r@ ~previous-holder !
  #>entity r@ ~holder !
  r@ ~owner !
  r@ ~bitfields /bitfields 1- 0 do  tuck i + c!  -1 +loop drop
  r@ ~times-open !
  r@ ~conversations !
  \ 2dup cr type .s  \ XXX INFORMER
  r> name! ;
  \ Restaura los datos de un ente cuyo número ordinal es _u_.  _x0 ...
  \ xn_ son los datos del ente, en orden inverso a como los crea la
  \ palabra `save-entity`.
  \ XXX TODO -- reescribir, recuperando en bruto todas las celdas del
  \ registro, sin distinguir campos

: load-plot ( x0 ... xn -- )
  recent-talks-to-the-leader !
  sword-forbidden? !
  stone-forbidden? !
  hacked-the-log? !
  climbed-the-fallen-away? !
  battle# !
  ambrosio-follows? ! ;
  \ Restaura las variables de la trama.
  \ Debe hacerse en orden alfabético inverso.

restore-wordlists

: string>file ( ca len -- )
  s\" s\" " 2swap s+ s\" \"" s+ >file ;
  \ Escribe una cadena en el fichero de la partida.

: f>string ( f -- ca len )
  if  s" true"  else  s" false"  then ;
  \ Convierte un indicador binario en su nombre de constante.

: f>file ( f -- ) f>string >file ;
  \ Escribe un indicador binario en el fichero de la partida.

: n>string ( n -- ca len )
  s>d swap over dabs <# #s rot sign #> >stringer ;
  \ Convierte un número con signo en una cadena.

: u>string ( u -- ca len ) s>d <# #s #> >stringer ;
  \ Convierte un número sin signo en una cadena.

: 00>s ( u -- ca1 len1 ) s>d <# # #s #> >stringer ;
  \ Convierte un número sin signo en una cadena (de dos dígitos como mínimo).

: 00>s+ ( u ca1 len1 -- ca2 len2 ) rot 00>s s+ ;
  \ Añade a una cadena un número tras convertirlo en cadena.

: yyyy-mm-dd-hh:mm:ss$ ( -- ca len )
  time&date 00>s hyphen+ 00>s+ hyphen+ 00>s+ space+
  00>s+ colon+ 00>s+ colon+ 00>s+ ;
  \ Devuelve la fecha y hora actuales como una cadena en formato
  \ «aaaa-mm-dd-hh:mm:ss».

: n>file ( n -- ) n>string >file ;
  \ Escribe un número con signo en el fichero de la partida.

: entity>file ( a -- ) entity># n>file ;
  \ Escribe la referencia a un ente _a_ en el fichero de la partida.
  \ Esta palabra es necesaria porque no es posible guardar y restaurar
  \ las direcciones de ficha de los entes, pues variarán con cada
  \ sesión de juego. Hay que guardar los números ordinales de las
  \ fichas y con ellos calcular sus direcciones durante la restauración.

: save-entity ( u -- )
  dup #>entity >r
  r@ name string>file
  r@ conversations n>file
  r@ times-open n>file
  r@ ~bitfields /bitfields bounds do  i c@ n>file  loop
  r@ owner n>file
  r@ holder entity>file
  r@ previous-holder entity>file
  r@ visits n>file
  r@ familiar n>file
  r@ north-exit entity>file
  r@ south-exit entity>file
  r@ east-exit entity>file
  r@ west-exit entity>file
  r@ up-exit entity>file
  r@ down-exit entity>file
  r@ out-exit entity>file
  r@ in-exit entity>file
  r> direction n>file
  n>file  \ Número ordinal del ente
  s" load-entity" >file/  ;  \ Palabra que hará la restauración del ente
  \ Escribe los datos de un ente (cuyo número ordinal es _u_) en el
  \ fichero de la partida.
  \ XXX TODO -- reescribir, grabando en bruto todas las celdas del
  \ registro, sin distinguir campos

: rule>file ( -- )
  s" \ ----------------------------------------------------" >file/ ;
  \ Escribe una línea de separación en el fichero de la partida.

: section>file ( ca len -- )
  cr>file rule>file s" \ " >file >file/ rule>file cr>file ;
  \ Escribe el título de una sección en el fichero de la partida.

: save-entities ( -- )
  s" Entes" section>file #entities 0 do  i save-entity  loop ;
  \ Escribe los datos de los entes en el fichero de la partida.

: save-config ( -- ) s" Configuración" section>file ;
  \ Escribe los valores de configuración en el fichero de la partida.
  \ XXX TODO

: save-plot ( -- )
  s" Trama" section>file
  ambrosio-follows? @ f>file
  battle# @ n>file
  climbed-the-fallen-away? @ f>file
  hacked-the-log? @ f>file
  stone-forbidden? @ f>file
  sword-forbidden? @ f>file
  recent-talks-to-the-leader @ n>file
  s" load-plot" >file/ ;
  \ Escribe las variables de la trama en el fichero de la partida.
  \ Debe hacerse en orden alfabético.  Escribe también la palabra que
  \ hará la restauración de la trama

: file-header ( -- )
  s" \ Datos de restauración de una partida de «Asalto y castigo»"
  >file/
  s" \ (http://pragramandala.net/es.programa.asalto_y_castigo.forth.html)"
  >file/
  s" \ Fichero creado en" yyyy-mm-dd-hh:mm:ss$ txt+ >file/ ;
  \ Escribe la cabecera del fichero de la partida.

: write-game-file ( -- )
  file-header save-entities save-config save-plot ;
  \ Escribe el contenido del fichero de la partida.

: fs+ ( ca1 len1 -- ca2 len2 ) s" .fs" s+ ;
  \ Añade la extensión «.fs» a un nombre de fichero _ca1 len1_.

: ?fs+ ( ca1 len1 -- ca1 len1 | ca2 len2 )
  s" .fs" ends? ?exit  fs+ ;
  \ Añade la extensión «.fs» a un nombre de fichero _ca1 len1_,
  \ si no la tiene ya.

: (save-the-game) ( ca len -- )
  ?fs+ create-game-file write-game-file close-game-file ;
  \ Salva la partida.

: save-the-game ( ca len -- )
  \ ?no-main-complement \ XXX TODO
  (save-the-game) ;
  \ Acción de salvar la partida.

: continue-the-loaded-game ( -- )
  scene-break new-page
  my-holder describe-location ;
  \ Continúa el juego en el punto que se acaba de restaurar.

: load-the-game ( ca len -- )
  \ ?no-main-complement  \ XXX TODO
  restore-wordlist 1 set-order
  [debug-filing] [??] ~~
  \ included  \ XXX FIXME -- el sistema estalla
  \ 2drop  \ XXX NOTE: sin error
  \ cr type  \ XXX NOTE: sin error
  2>r save-input 2r>
  [debug-filing] [??] ~~
  ?fs+
  [debug-filing] [??] ~~
[false] [if]  \ XXX INFORMER
  read-game-file
[else]
  ['] read-game-file
  [debug-filing] [??] ~~
  catch
  [debug-filing] [??] ~~
  restore-wordlists
  [debug-filing] [??] ~~
  ?dup if
   ( ca len u2 ) nip nip
    case  \ XXX TMP
      2 of  s" Fichero no encontrado." narrate  endof
      s" Error al intentar leer el fichero." narrate
    endcase
    [debug-filing] [??] ~~
  then
[then]
  [debug-filing] [??] ~~
  restore-input
  if
    \ XXX TMP
    s" Error al intentar restaurar la entrada tras leer el fichero." narrate
  then
  [debug-filing] [??] ~~
  continue-the-loaded-game ;
  \ Acción de salvar la partida.
  \ XXX FIXME -- no funciona bien?
  \ XXX TODO -- probarlo

\ ----------------------------------------------
\ Acciones de configuración

: recolor ( -- )
  init-colors  new-page  my-holder describe ;
  \ XXX TODO rename

defer finish ( -- )
  \ XXX TODO rename

\ ==============================================================
\ Change log

\ 2017-11-10: Update to Talanto 0.62.0: replace field notation
\ "location" with "holder".
\
\ 2017-11-16: Update to Galope 0.138.1: replace `++` with `1+!`, and
\ `?++` with `?1+!`.

\ vim:filetype=gforth:fileencoding=utf-8
