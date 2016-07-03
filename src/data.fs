\ data.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607040046

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Atributos y descripciones de entes

\ ----------------------------------------------
\ Ente protagonista

: describe-ulfius  ( -- )
  s" Sientes sobre ti la carga de tanto"
  s{ s" sucedido" s" acontecido" s" acontecido" s" vivido" }s&
  period+ paragraph  ;

ulfius~ :init  ( -- )
  ['] describe-ulfius self~ be-description-xt
  s" Ulfius" self~ ms-name!
  self~ be-human
  self~ have-personal-name
  self~ have-no-article
  location-01~ self~ be-there  ;

\ ----------------------------------------------
\ Entes personaje

: describe-ambrosio  ( -- )
  ambrosio~ conversations
  if    s" Ambrosio"
        s" es un hombre de mediana edad, que te mira afable." s&
  else  s" Es de mediana edad y mirada afable."
  then  paragraph  ;

ambrosio~ :init  ( -- )
  ['] describe-ambrosio self~ be-description-xt
  s" hombre" self~ ms-name!
  self~ be-character
  self~ be-human
  location-19~ self~ be-there  ;
  \ N.B. El nombre cambiará a «Ambrosio» durante el juego.

: describe-leader  ( -- )
  leader~ conversations?
  if    s" Es el jefe de los refugiados."
  else  s" Es un anciano."
  then  paragraph  ;
  \ XXX TODO -- elaborar esto según la trama

leader~ :init  ( -- )
  ['] describe-leader self~ be-description-xt
  s" anciano" self~ ms-name!
  self~ be-character
  self~ be-human
  self~ be-not-listed
  location-28~ self~ be-there  ;

: describe-soldiers  ( -- )
  true case
    still-in-the-village? of  soldiers-steal-spite-of-officers  endof
\   back-to-the-village? of  soldiers-go-home  endof
      \ XXX TODO -- no usado
    pass-still-open? of  soldiers-go-home  endof
\   battle? of  battle-phase  endof
    \ XXX TODO -- no usado. redundante, porque tras la descripción se
    \ mostrará otra vez la situación de la batalla
  endcase  ;
  \ Describe a tus soldados.

soldiers~ :init  ( -- )
  ['] describe-soldiers self~ be-description-xt
  s" soldados" self~ mp-name!
  self~ be-human
  self~ familiar++
  self~ be-decoration
  self~ belongs-to-protagonist  ;

: describe-officers  ( -- )
  true case
    still-in-the-village? of  ^officers-forbid-to-steal$  endof
\   back-to-the-village? of  officers-go-home  endof
    \ XXX TODO -- no usado
    pass-still-open? of  officers-go-home  endof
\   battle? of  battle-phase  endof
    \ XXX TODO -- no usado. redundante, porque tras la descripción se
    \ mostrará otra vez la situación de la batalla
  endcase  ;
  \ Describe a tus soldados.

officers~ :init  ( -- )
  ['] describe-officers self~ be-description-xt
  s" oficiales" self~ mp-name!
  self~ be-human
  self~ familiar++
  self~ be-decoration
  self~ belongs-to-protagonist  ;

: describe-refugees  ( -- )
  my-location case
  location-28~ of  refugees-description  endof
  location-29~ of  s" Todos los refugiados quedaron atrás."
                   paragraph  endof
  endcase  ;
  \ XXX TODO -- ampliar el texto

refugees~ :init  ( -- )
  ['] describe-refugees self~ be-description-xt
  s" refugiados" self~ mp-name!
  self~ be-human
  self~ be-decoration  ;

\ ----------------------------------------------
\ Entes objeto

: describe-altar  ( -- )
  s" Está" s{ s" situado" s" colocado" }s&
  s" justo en la mitad del puente." s&
  idol~ is-unknown?
  if  s" Debe de sostener algo importante." s&   then
  paragraph  ;

altar~ :init  ( -- )
  ['] describe-altar self~ be-description-xt
  s" altar" self~ ms-name!
  self~ be-decoration
  impossible-error# self~ ~take-error# !
  location-18~ self~ be-there  ;

: describe-arch  ( -- )
  s" Un sólido arco de piedra, de una sola pieza."
  paragraph  ;
  \ XXX TODO -- mejorar texto

arch~ :init  ( -- )
  ['] describe-arch self~ be-description-xt
  s" arco" self~ ms-name!
  self~ be-decoration
  location-18~ self~ be-there  ;

: describe-bed  ( -- )
  s{ s" Parece poco" s" No tiene el aspecto de ser muy"
  s" No parece especialmente" }s
  s{ s" confortable" s" cómod" bed~ adjective-ending s+ }s& period+
  paragraph  ;

bed~ :init  ( -- )
  ['] describe-bed self~ be-description-xt
  s" catre" self~ ms-name!
  location-46~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-bridge  ( -- )
  s" Está semipodrido."
  paragraph  ;
  \ XXX TODO -- mejorar texto

bridge~ :init  ( -- )
  ['] describe-bridge self~ be-description-xt
  s" puente" self~ ms-name!
  self~ be-decoration
  location-13~ self~ be-there  ;

: describe-candles  ( -- )
  s" Están muy consumidas."
  paragraph  ;

candles~ :init  ( -- )
  ['] describe-candles self~ be-description-xt
  s" velas" self~ fp-name!
  location-46~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-cave-entrance  ( -- )
  the-cave-entrance-is-hidden$
  you-were-lucky-discovering-it$ s&
  it's-your-last-hope$ s&
  paragraph  ;

cave-entrance~ :init  ( -- )
  ['] describe-cave-entrance self~ be-description-xt
  s" entrada a una cueva" self~ fs-name!  ;

: describe-cloak  ( -- )
  s" Tu capa de general, de fina lana"
  s{ s" tintada de negro." s" negra." }s&
  paragraph  ;

cloak~ :init  ( -- )
  ['] describe-cloak self~ be-description-xt
  s" capa" self~ fs-name!
  self~ be-cloth
  self~ belongs-to-protagonist
  self~ be-worn
  self~ taken  ;

cuirasse~ :init  ( -- )
  \ ['] describe-cuirasse self~ be-description-xt
  s" coraza" self~ fs-name!
  self~ be-cloth
  self~ belongs-to-protagonist
  self~ taken
  self~ be-worn  ;

\ defer lock-found  ( -- )  \ XXX OLD
  \ Encontrar el candado.

: describe-door  ( -- )
  door~ times-open if  s" Es"  else  s" Parece"  then
  s" muy" s?& s{ s" recia" s" gruesa" s" fuerte" }s&
  location-47~ am-i-there? if
    lock~ is-known?
    if    s" . A ella está unido el candado"
    else  s"  y tiene un gran candado"
    then  s+ lock-found
  then  period+
  s" Está" s& door~ «open»|«closed» s& period+ paragraph  ;

door~ :init  ( -- )
  ['] describe-door self~ be-description-xt
  s" puerta" self~ fs-name!
  self~ be-closed
  impossible-error# self~ ~take-error# !
  location-47~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-emerald  ( -- )
  s" Es preciosa."
  paragraph  ;

emerald~ :init  ( -- )
  ['] describe-emerald self~ be-description-xt
  s" esmeralda" self~ fs-name!
  location-39~ self~ be-there  ;

: describe-fallen-away  ( -- )
  s{
    s" Muchas," s" Muchísimas," s" Numerosas,"
    s" Un gran número de" s" Una gran cantidad de"
    \ XXX TODO -- si se añade lo que sigue,
    \ hay que crear los entes "pared" y "muro":
    \ s" Un muro de" s" Una pared de"
  }s
  s{ s" inalcanzables" s" inaccesibles" }s&
  s{ s" y enormes" s" y pesadas" s" y grandes" }s?&
  s" rocas," s& s{ s" apiladas" s" amontonadas" }s&
  s{
    s" una sobre otra"
    s" unas sobre otras"
    s" una encima de otra"
    s" unas encima de otras"
    s" la una encima de la otra"
    s" las unas encima de las otras"
    s" la una sobre la otra"
    s" las unas sobre las otras"
  }s& period+
  paragraph  ;

fallen-away~ :init  ( -- )
  ['] describe-fallen-away self~ be-description-xt
  s" derrumbe" self~ ms-name!
  self~ be-decoration
  nonsense-error# self~ ~take-error# !
  location-09~ self~ be-there  ;

: don't-take-the-flags  ( -- )
  s" No hay" s{ s" motivo" s" razón" s" lugar" }s&
  s" para ofender a"
  talked-to-the-leader?
  if    s" los refugiados."
  else  s" nadie."
  then  s& narrate  ;

: describe-flags  ( -- )
  s" Son las banderas britana y sajona."
  s" Dos dragones rampantes, rojo y blanco respectivamente, enfrentados." s&
  paragraph  ;

flags~ :init  ( -- )
  ['] describe-flags self~ be-description-xt
  s" banderas" self~ fp-name!
  self~ be-decoration
  ['] don't-take-the-flags self~ ~take-error# !
  location-28~ self~ be-there  ;

: describe-flint  ( -- )
  s" Es una piedra dura y afilada."
  paragraph  ;

flint~ :init  ( -- )
  ['] describe-flint self~ be-description-xt
  s" pedernal" self~ ms-name!  ;

: describe-grass  ( -- )
  door~ times-open if
    s" Está" grass~ verb-number-ending+
    s" aplastad" grass~ adjective-ending+ s&
    s{ s" en el" s" bajo el" s" a lo largo del" }s&
    s{ s" trazado" s" recorrido" }s&
    s{ s" de la puerta." s" que hizo la puerta al abrirse." }s&
  else
    s" Cubre" grass~ verb-number-ending+
    s" el suelo junto a la puerta, lo" s&
    s{ s" que" s" cual" }s&
    s{ s" indica" s" significa" s" delata" }s
    s" que ésta" s&
    s{ s" no ha sido abierta en" s" lleva cerrada"
    s" ha permanecido cerrada" s" durante" s?& }s
    s" mucho tiempo." s&
  then  paragraph  ;

grass~ :init  ( -- )
  ['] describe-grass self~ be-description-xt
  s" hierba" self~ fs-name!
  self~ be-decoration  ;

: describe-idol  ( -- )
  s" El ídolo tiene dos agujeros por ojos."
  paragraph  ;

idol~ :init  ( -- )
  ['] describe-idol self~ be-description-xt
  s" ídolo" self~ ms-name!
  self~ be-decoration
  impossible-error# self~ ~take-error# !
  location-41~ self~ be-there  ;

: describe-key  ( -- )
  s" Es una llave grande, de hierro herrumboso."
  paragraph  ;

key~ :init  ( -- )
  ['] describe-key self~ be-description-xt
  s" llave" self~ fs-name!
  location-46~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-lake  ( -- )
  s{ s" La" s" Un rayo de" }s
  s" luz entra por un resquicio," s&
  s" y sus caprichosos reflejos te maravillan." s&
  paragraph  ;

lake~ :init  ( -- )
  ['] describe-lake self~ be-description-xt
  s" lago" self~ ms-name!
  self~ be-decoration
  nonsense-error# self~ ~take-error# !
  location-44~ self~ be-there  ;

: describe-lock  ( -- )
  s" Es grande y parece" s{ s" fuerte." s" resistente." }s&
  s" Está" s&{ s" fijad" s" unid" }s& lock~ adjective-ending+
  s" a la puerta y" s&
  lock~ «open»|«closed» s& period+
  paragraph  ;

lock~ :init  ( -- )
  ['] describe-lock self~ be-description-xt
  s" candado" self~ ms-name!
  self~ be-decoration
  self~ be-closed
  impossible-error# self~ ~take-error# !
  self~ ambrosio~ be-owner  ;

: describe-log  ( -- )
  s" Es un tronco"
  s{ s" recio," s" resistente," s" fuerte," }s&
  but$ s& s{ s" liviano." s" ligero." }s&
  paragraph  ;

log~ :init  ( -- )
  ['] describe-log self~ be-description-xt
  s" tronco" self~ ms-name!
  location-15~ self~ be-there  ;

: describe-piece  ( -- )
  s" Un pequeño" s{ s" retal" s" pedazo" s" trozo" s" resto" }s&
  of-your-ex-cloak$ s&
  paragraph  ;

piece~ :init  ( -- )
  ['] describe-piece self~ be-description-xt
  s" trozo de tela" self~ ms-name!  ;
  \ XXX TODO -- ojo con este «de tela»: «tela» es sinónimo de trozo;
  \ hay que contemplar estos casos en el cálculo de los genitivos.

: describe-rags  ( -- )
  s" Un" s{ s" retal" s" pedazo" s" trozo" }s&
  s{ s" un poco" s" algo" }s?& s" grande" s&
  of-your-ex-cloak$ s&
  paragraph  ;

rags~ :init  ( -- )
  ['] describe-rags self~ be-description-xt
  s" harapo" self~ ms-name!  ;

: describe-ravine-wall  ( -- )
  s" en" the-cave-entrance-was-discovered? ?keep
  s" la pared" s& rocky(f)$ s& ^uppercase
  the-cave-entrance-was-discovered? if
    s" , que" it-looks-impassable$ s& comma+ s?+
    the-cave-entrance-is-visible$ s&
    period+ paragraph
  else
    it-looks-impassable$ s&
    ravine-wall~ is-known?
    if    you-maybe-discover-the-cave-entrance
    else  period+ paragraph
    then
  then  ;

ravine-wall~ :init  ( -- )
  ['] describe-ravine-wall self~ be-description-xt
  s" pared" rocky(f)$ s& self~ fs-name!
  location-08~ self~ be-there
  self~ be-not-listed  \ XXX OLD -- innecesario
  self~ be-decoration  ;

: describe-rocks  ( -- )
  location-31~ has-north-exit?
  if  (rocks)-on-the-floor$ ^uppercase
  else  (rocks)-clue$
  then  period+ paragraph  ;

rocks~ :init  ( -- )
  ['] describe-rocks self~ be-description-xt
  s" rocas" self~ fp-name!
  self~ be-decoration
  location-31~ self~ be-there  ;

: describe-snake  ( -- )
  s" Una serpiente grande, muy atenta a tu más mínimo movimiento."
  paragraph  ;

snake~ :init  ( -- )
  ['] describe-snake self~ be-description-xt
  s" serpiente" self~ fs-name!
  self~ be-animal
  dangerous-error# self~ ~take-error# !
  location-43~ self~ be-there  ;
  \ XXX TODO -- distinguir si está muerta; en el programa original no
  \ hace falta

: describe-stone  ( -- )
  s" Recia y pesada, pero no muy grande, de forma piramidal."
  paragraph  ;

stone~ :init  ( -- )
  ['] describe-stone self~ be-description-xt
  s" piedra" self~ fs-name!
  location-18~ self~ be-there  ;

: describe-sword  ( -- )
  s{ s" Legado" s" Herencia" }s s" de tu padre," s&
  s{ s" fiel herramienta" s" arma fiel" }s& s" en" s&
  s{ s" mil" s" incontables" s" innumerables" }s&
  s" batallas." s&
  paragraph  ;

sword~ :init  ( -- )
  ['] describe-sword self~ be-description-xt
  s" espada" self~ fs-name!
  self~ belongs-to-protagonist
  self~ taken  ;

: describe-table  ( -- )
  s" Es pequeña y de" s{ s" basta" s" tosca" }s& s" madera." s&
  paragraph  ;

table~ :init  ( -- )
  ['] describe-table self~ be-description-xt
  s" mesa" self~ fs-name!
  location-46~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-thread  ( -- )
  s" Un hilo" of-your-ex-cloak$ s&
  paragraph  ;

thread~ :init  ( -- )
  ['] describe-thread self~ be-description-xt
  s" hilo" self~ ms-name!  ;

: describe-torch  ( -- )
  s" Está" torch~ is-lit?
  if  s" encendida."  else  s" apagada."  then  s& paragraph  ;

torch~ :init  ( -- )
  ['] describe-torch self~ be-description-xt
  s" antorcha" self~ fs-name!
  self~ be-light
  self~ be-not-lit  ;

: describe-waterfall  ( -- )
  s" No ves nada por la cortina de agua."
  s" El lago es muy poco profundo." s&
  paragraph  ;

waterfall~ :init  ( -- )
  ['] describe-waterfall self~ be-description-xt
  s" cascada" self~ fs-name!
  self~ be-decoration
  nonsense-error# self~ ~take-error# !
  location-38~ self~ be-there  ;

\ ----------------------------------------------
\ Entes escenario

\ Las palabras que describen entes escenario reciben en `sight`
\ (variable que está creada con `value` y por tanto devuelve su
\ valor como si fuera una constante) un identificador de ente.
\ Puede ser el mismo ente escenario o un ente de dirección.  Esto
\ permite describir lo que hay más allá de cada escenario en
\ cualquier dirección.

: describe-location-01  ( -- )
  sight case
  my-location of
    s" No ha quedado nada en pie, ni piedra sobre piedra."
    s{ s" El entorno es desolador." s" Todo alrededor es desolación." }s
    rnd2swap s&
    s{ ^only$ remains$ s&
    s" Lo único que" remains$ s& s" por hacer" s?& s" es" s&
    s" No" remains$ s& s{ s" más" s" otra cosa" }s& s" que" s&
    }s& to-go-back$ s& s" al sur, a casa." s&
    paragraph
    endof
  south~ of
    2 random if \ Versión 0:
      ^toward-the(m)$ s" sur" s&
      s{ s" está" s" puedo ver" s" se puede ver" }s&
      s" la colina." s&  \ Descripción principal
      s" Y mucho más allá está tu" home$ s& period+  \ Coletilla...
      2 random * s&  \ ...que aparecerá con un 50% de probabilidad
    else  \ Versión 1:
      s" Muy lejos de aquí está tu" home$ s& comma+
      s" y el camino empieza detrás de aquella colina." s&
    then  paragraph
    endof
  up~ of
    s{ s" pronto" s" sin compasión" s" de inmediato" }s
    s{ s" vencidas" s" derrotadas" s" sojuzgadas" }s rnd2swap s& ^uppercase
    s" por la fría" s&
    s{ s" e implacable" s" y despiadada" }s?&
    s" niebla," s& s" torpes" s" tristes" both?&
    s" columnas de" s& s" negro" s" humo" rnd2swap s& s&
    (they)-go-up$ s&
    s{ s" lastimosamente" s" penosamente" }s&
    s" hacia" s{ s" el cielo" s" las alturas" }s& s?&
    s{ s" desde" s" de entre" }s& rests-of-the-village$ s&
    s" , como si" s" también" s" ellas" rnd2swap s& s?&
    s{ s" desearan" s" anhelaran" s" soñaran" }s&
    s" poder hacer un último esfuerzo por" s?&
    s" escapar" s& but|and$ s& s" no supieran cómo" s& s?+
    s" ..." s+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-01  ( -- )
  soldiers~ be-here
  still-in-the-village?
  if  celebrating  else  going-home  then  ;

location-01~ :init  ( -- )
  ['] describe-location-01 self~ be-description-xt
  ['] after-describing-location-01 self~ be-after-describing-location-xt
  s" aldea sajona" self~ fs-name!
  0 location-02~ 0 0 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear colina en los tres primeros escenarios

: describe-location-02  ( -- )
  sight case
  my-location of
    s" Sobre" s" la cima de" s?&
    s" la colina, casi" s& s{ s" sobre" s" por encima de" }s&
    s" la" s&
    s" espesa" s" fría" both?& s" niebla de la aldea sajona arrasada al norte, a tus pies." s&
    ^the-path$ s& goes-down$ s& toward-the(m)$ s& s" oeste." s&
    paragraph
    endof
  north~ of
    s" La" poor-village$ s& s" sajona" s& s" , arrasada," s?+ s" agoniza bajo la" s&
    s" espesa" s" fría" both?& s" niebla." s&
    paragraph
    endof
  west~ of
    ^the-path$ goes-down$ s& s" por la" s& s" ladera de la" s?& s" colina." s&
    paragraph
    endof
  down~ of
    location-02~ down-exit location-02~ north-exit =
    if  north~  else  west~  then  describe
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-02  ( -- )
  \ Decidir hacia dónde conduce la dirección hacia abajo
  [false] [if]  \ XXX OLD -- Primera versión
    \ Decidir al azar:
    location-02~ location-01~ location-03~ 2 choose d-->
  [else]  \ XXX NEW -- Segunda versión mejorada
    \ Decidir según el escenario de procedencia:
    location-02~
    protagonist~ previous-location location-01~ =  \ ¿Venimos de la aldea?
    if  location-03~  else  location-01~  then  d-->
  [then]
  soldiers~ be-here going-home  ;

location-02~ :init  ( -- )
  ['] describe-location-02 self~ be-description-xt
  ['] after-describing-location-02 self~ be-after-describing-location-xt
  s" cima de la colina" self~ fs-name!
  location-01~ 0 0 location-03~ 0 0 0 0 self~ init-location  ;

  \ N.B. Desde el escenario 02, uno puede bajar por el sur o por el
  \ oeste; esto se decide al azar cada vez que se entra en el
  \ escenario. Por ello descripción tiene esto en cuenta y
  \ redirige a la descripción adecuada.


  \ XXX TODO -- crear entes en escenario 2: aldea, niebla

: describe-location-03  ( -- )
  sight case
  my-location of
    ^the-path$ s" avanza por el valle," s&
    s" desde la parte alta, al este," s&
    s" a una zona" s& very-or-null$ s& s" boscosa, al oeste." s&
    paragraph
    endof
  east~ of
    ^the-path$ s" se pierde en la parte alta del valle." s&
    paragraph
    endof
  west~ of
    s" Una zona" very-or-null$ s& s" boscosa." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-03~ :init  ( -- )
  ['] describe-location-03 self~ be-description-xt
  ['] soldiers-are-here self~ be-after-describing-location-xt
  s" camino entre colinas" self~ ms-name!
  0 0 location-02~ location-04~ 0 0 0 0 self~ init-location  ;

: describe-location-04  ( -- )
  sight case
  my-location of
    s" Una senda parte al oeste, a la sierra por el paso del Perro,"
    s" y otra hacia el norte, por un frondoso bosque que la rodea." s&
    paragraph
    endof
  north~ of
    ^a-path$ surrounds$ s& s" la sierra a través de un frondoso bosque." s&
    paragraph
    endof
  west~ of
    ^a-path$ leads$ s& toward-the(f)$ s& s" sierra por el paso del Perro." s&
    paragraph
    endof
  down~ of  endof
  up~ of  endof
  uninteresting-direction
  endcase  ;

location-04~ :init  ( -- )
  ['] describe-location-04 self~ be-description-xt
  ['] soldiers-are-here self~ be-after-describing-location-xt
  s" cruce de caminos" self~ ms-name!
  location-05~ 0 location-03~ location-09~ 0 0 0 0 self~ init-location  ;

: describe-location-05  ( -- )
  sight case
  my-location of
    ^toward-the(m)$ s" oeste se extiende" s&
    s{ s" frondoso" s" exhuberante" }s& \ XXX TODO -- independizar
    s" el bosque que rodea la sierra." s&
    s" La salida se abre" s&
    toward-the(m)$ s& s" sur." s&
    paragraph
    endof
  south~ of
    s" Se ve la salida del bosque."
    paragraph
    endof
  west~ of
    s" El bosque se extiende"
    s{ s" exhuberante" s" frondoso" }s&
    s" alrededor de la sierra." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-05~ :init  ( -- )
  ['] describe-location-05 self~ be-description-xt
  ['] soldiers-are-here self~ be-after-describing-location-xt
  s" linde del bosque" self~ fs-name!
  0 location-04~ 0 location-06~ 0 0 0 0 self~ init-location  ;

: describe-location-06  ( -- )
  sight case
  my-location of
    s" Jirones de niebla se enzarcen en frondosas ramas y arbustos."
    ^the-path$ s& s" serpentea entre raíces, de un luminoso este" s&
    toward-the(m)$ s& s" oeste." s&
    paragraph
    endof
  east~ of
    s" De la linde del bosque"
    s{ s" procede" s" llega" s" viene" }s&
    s{ s" una cierta" s" algo de" s" un poco de" }s&
    s{ s" claridad" s" luminosidad" }s&
    s" entre" s&
    s{ s" el follaje" s" la vegetación" }s& period+
    paragraph
    endof
  west~ of
    s" La niebla parece más" s" densa" s" oscura" both?& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-06~ :init  ( -- )
  ['] describe-location-06 self~ be-description-xt
  ['] soldiers-are-here self~ be-after-describing-location-xt
  s" bosque" self~ ms-name!
  0 0 location-05~ location-07~ 0 0 0 0 self~ init-location  ;

: describe-location-07  ( -- )
  sight case
  my-location of
    s" Abruptamente, el bosque desaparece y deja paso a un estrecho camino entre altas rocas."
    s" El" s& s{ s" inquietante" s" sobrecogedor" }s&
    s" desfiladero" s& s{ s" tuerce" s" gira" }s&
    s" de este a sur." s&
    paragraph
    endof
  south~ of
    ^the-path$ s" gira" s& in-that-direction$ s& period+
    paragraph
    endof
  east~ of
    s" La estrecha senda es" s{ s" engullida" s" tragada" }s&
    s" por las" s&
    s" fauces" s{ s" frondosas" s" exhuberantes" }s rnd2swap s& s&
    s" del bosque." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-07~ :init  ( -- )
  ['] describe-location-07 self~ be-description-xt
  ['] soldiers-are-here self~ be-after-describing-location-xt
  s" paso del Perro" self~ ms-name!
  0 location-08~ location-06~ 0 0 0 0 0 self~ init-location  ;

: describe-location-08  ( -- )
  sight case
  my-location of
    ^the-pass-way$ s" entre el desfiladero sigue de norte a este" s&
    s" junto a una" s&
    s{  s" pared" rocky(f)$ s& s" rocosa pared" }s& period+
      \ XXX TODO -- completar con entrada a caverna, tras ser descubierta
    paragraph
    endof
  north~ of
    s" El camino" s{ s" tuerce" s" gira" }s&
      \ XXX TODO -- independizar gira/tuerce
    s" hacia el inquietante paso del Perro." s&
    paragraph
    endof
  south~ of
    s{ ^in-that-direction$ s" Hacia el sur" }s
    s{ s" se alza" s" se levanta" }s&
    \ s" una pared" s& rocky(f)$ s& \ XXX OLD
    ravine-wall~ full-name s&
    the-cave-entrance-was-discovered? if
      comma+ s" en la" s&{ s" que" s" cual" }s&
      the-cave-entrance-is-visible$ s&
      period+ paragraph
    else
      ravine-wall~ is-known? if
        s" que" it-looks-impassable$ s& s?&
        you-maybe-discover-the-cave-entrance
      else
        period+ paragraph  ravine-wall~ familiar++
      then
    then
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-08  ( -- )
  soldiers-are-here pass-still-open? ?? ambush  ;

location-08~ :init  ( -- )
  ['] describe-location-08 self~ be-description-xt
  ['] after-describing-location-08 self~ be-after-describing-location-xt
  s" desfiladero" self~ ms-name!
  location-07~ 0 0 0 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear pared y roca y desfiladero en escenario 08

: describe-location-09  ( -- )
  sight case
  my-location of
    ^the-path$ goes-down$ s& s" hacia la agreste sierra, al oeste," s&
    s" desde los" s& s" verdes" s" valles" rnd2swap s& s& s" al este." s&
    ^but$ s& s" un" s&{ s" gran" s" enorme" }s?& s" derrumbe" s&
    (it)-blocks$ s& s" el paso hacia" s&{ s" el oeste" s" la sierra." }s&
    paragraph
    endof
  east~ of
    ^can-see$ s" la salida del bosque." s&
    paragraph
    endof
  west~ of
    s" Un gran derrumbe" (it)-blocks$ s& the-pass$ s&
    toward$ s& s" la sierra." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-09~ :init  ( -- )
  ['] describe-location-09 self~ be-description-xt
  ['] soldiers-are-here self~ be-after-describing-location-xt
  s" derrumbe" self~ ms-name!
  0 0 location-04~ 0 0 0 0 0 self~ init-location  ;

: describe-location-10  ( -- )
  sight case
  my-location of
    s" El estrecho paso se adentra hacia el oeste, desde la boca, al norte."
    paragraph
    endof
  north~ of
    s" La boca de la gruta conduce al exterior."
    paragraph
    endof
  east~ of
  endof
  uninteresting-direction
  endcase  ;

: after-describing-location-10  ( -- )
  s" entrada a la cueva" cave-entrance~ fs-name!
  cave-entrance~ familiar++
  location-08~ my-previous-location = if  \ Venimos del exterior
    location-10~ visits
    if  ^again$  else  ^finally$ s" ya" s?&  then
    \ XXX TODO -- ampliar con otros textos alternativos
    you-think-you're-safe$ s&
    but-it's-an-impression$ s?+
    period+ narrate
  \ XXX TODO -- si venimos del interior, mostrar otros textos
  then  ;

location-10~ :init  ( -- )
  ['] describe-location-10 self~ be-description-xt
  ['] after-describing-location-10 self~ be-after-describing-location-xt
  s" gruta de entrada" self~ fs-name!
  self~ be-indoor-location
  location-08~ 0 0 location-11~ 0 0 0 0 self~ init-location  ;

: describe-location-11  ( -- )
  sight case
  my-location of
    s" Una" s{
      s{ s" gran" s" amplia" }s s" estancia" s&
      s" estancia" s" muy" s?& s{ s" grande" s" amplia" }s&
    }s& s" alberga un lago de" s&
    s{
      s" profundas" s" aguas" rnd2swap s& comma+ s" e" s?& s" iridiscentes" s&
      s" aguas tan profundas como iridiscentes,"
    }s&
    s{ s" gracias a" s" debido a" s" a causa de" s" por el efecto de" }s&
    s{
      s" la" s{ s" débil" s" tenue" }s?& s" luz" s&
        s{  s" que se filtra" s{ s" del" s" desde el" }s&
            s{ s" procendente" s" que procede" s" que entra" }s s" del" s&
        }s
      s" los" s{ s" débiles" s" tenues" }s?& s" rayos de luz" s&
        s{  s" que se filtran" s{ s" del" s" desde el" }s&
            s{ s" procendentes" s" que proceden" s" que entran" }s s" del" s&
        }s
    }s?& s" exterior." s&
    s" No hay" s&{ s" otra" s" más" }s& s" salida que el este." s&
    paragraph
    endof
  east~ of
    s" De la entrada de la gruta"
    s{ s" procede" s" proviene" }s&
    s" la" s& s{ s" luz" s" luminosidad" s" claridad" }s
    s" que hace brillar" s&
    s{ s" el agua" s" las aguas" s" la superficie" }s&
    s" del lago." s&
    paragraph
  endof
  uninteresting-direction
  endcase  ;

location-11~ :init  ( -- )
  ['] describe-location-11 self~ be-description-xt
  ['] lake-is-here self~ be-after-describing-location-xt
  s" gran lago" self~ ms-name!
  self~ be-indoor-location
  0 0 location-10~ 0 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. estancia y aguas en escenario 11

: describe-location-12  ( -- )
  sight case
  my-location of
    s" Una" s{ s" gran" s" amplia" }s&
    s" estancia se abre hacia el oeste," s&
    s" y se estrecha hasta" s& s{ s" morir" s" terminar" }s&
    s" , al este, en una" s+{ s" parte" s" zona" }s& s" de agua." s&
    paragraph
    endof
  east~ of
    s{ s" La estancia" s" El lugar" }s
    s" se estrecha hasta " s&
    s{ s" morir" s" terminar" }s&
    s" en una" s&{ s" parte" s" zona" }s& s" de agua." s&
    paragraph
  endof
  west~ of
    s" Se vislumbra la continuación de la cueva."
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-12~ :init  ( -- )
  ['] describe-location-12 self~ be-description-xt
  s" salida del paso secreto" self~ fs-name!
  self~ be-indoor-location
  0 0 0 location-13~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente agua en escentario 12

: describe-location-13  ( -- )
  sight case
  my-location of
    s" La sala se abre en"
    s{ s" semioscuridad" s" penumbra" }s&
    s" a un puente cubierto de podredumbre" s&
    s" sobre el lecho de un canal, de este a oeste." s&
    paragraph
    endof
  east~ of
    s" Se vislumbra el inicio de la cueva."
    paragraph
  endof
  west~ of
    s" Se vislumbra un recodo de la cueva."
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-13~ :init  ( -- )
  ['] describe-location-13 self~ be-description-xt
  s" puente semipodrido" self~ ms-name!
  self~ be-indoor-location
  0 0 location-12~ location-14~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. canal, agua, lecho(~catre) en escenario 14

: describe-location-14  ( -- )
  sight case
  my-location of
    s" La iridiscente cueva gira de este a sur."
    paragraph
    endof
  south~ of
    you-glimpse-the-cave$ paragraph
    endof
  east~ of
    you-glimpse-the-cave$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-14~ :init  ( -- )
  ['] describe-location-14 self~ be-description-xt
  s" recodo de la cueva" self~ ms-name!
  self~ be-indoor-location
  0 location-15~ location-13~ 0 0 0 0 0 self~ init-location  ;

: describe-location-15  ( -- )
  sight case
  my-location of
    s" La gruta" goes-down$ s& s" de norte a sur" s&
    s" sobre un lecho arenoso." s&
    s" Al este, un agujero del que llega" s&
    s{ s" algo de luz." s" claridad." }s&
    paragraph
    endof
  north~ of
    you-glimpse-the-cave$
    s" La cueva" goes-up$ s& in-that-direction$ s& period+
    paragraph
    endof
  south~ of
    you-glimpse-the-cave$
    s" La cueva" goes-down$ s& in-that-direction$ s& period+
    paragraph
    endof
  east~ of
    s{ s" La luz" s" Algo de luz" s" Algo de claridad" }s
    s" procede de esa dirección." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-15~ :init  ( -- )
  ['] describe-location-15 self~ be-description-xt
  s" pasaje arenoso" self~ ms-name!
  self~ be-indoor-location
  location-14~ location-17~ location-16~ 0 0 0 0 0 self~ init-location  ;

: describe-location-16  ( -- )
  sight case
  my-location of
    s" Como un acueducto, el agua"
    goes-down$ s& s" con gran fuerza de norte a este," s&
    s" aunque la salida practicable es la del oeste." s&
    paragraph
    endof
  north~ of
    s" El agua" goes-down$ s& s" con gran fuerza" s& from-that-way$ s& period+
    paragraph
    endof
  east~ of
    s" El agua" goes-down$ s& s" con gran fuerza" s& that-way$ s& period+
    paragraph
    endof
  west~ of
    s" Es la única salida." paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-16  ( -- )
  s" En la distancia, por entre los resquicios de las rocas,"
  s" y allende el canal de agua, los sajones" s&
  s{ s" intentan" s" se esfuerzan en" s" tratan de" s" se afanan en" }s&
  s{ s" hallar" s" buscar" s" localizar" }s&
  s" la salida que encontraste por casualidad." s&
  narrate  ;

location-16~ :init  ( -- )
  ['] describe-location-16 self~ be-description-xt
  ['] after-describing-location-16 self~ be-after-describing-location-xt
  s" pasaje del agua" self~ ms-name!
  self~ be-indoor-location
  0 0 0 location-15~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- el examen del agua aquí debe dar más pistas en
  \ escenario 16

: describe-location-17  ( -- )
  sight case
  my-location of
    s" Muchas estalactitas se agrupan encima de tu cabeza,"
    s" y se abren cual arco de entrada hacia el este y sur." s&
    paragraph
    endof
  north~ of
    you-glimpse-the-cave$
    paragraph
    endof
  up~ of
    s" Las estalactitas se agrupan encima de tu cabeza."
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-17~ :init  ( -- )
  ['] describe-location-17 self~ be-description-xt
  s" estalactitas" self~ fs-name!
  self~ be-indoor-location
  location-15~ location-20~ location-18~ 0 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. estalactitas en escenario 17

: describe-location-18  ( -- )
  sight case
  my-location of
    s" Un arco de piedra se extiende,"
    s{ s" cual" s" como si fuera un" s" a manera de" }s&
    s" puente" s&
    s" que se" s{ s" elevara" s" alzara" }s& s?&
    s{ s" sobre" s" por encima de" }s&
    s" la oscuridad, de este a oeste." s&
    s{ s" Hacia" s" En" }s& s" su mitad" s&
    altar~ is-known?
    if    s" está" s&
    else  s{ s" hay" s" es posible ver" s" puede verse" }s&
    then  altar~ full-name s& period+ paragraph
    endof
  east~ of
    s" El arco de piedra se extiende" that-way$ s& period+
    paragraph
    endof
  west~ of
    s" El arco de piedra se extiende" that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-18~ :init  ( -- )
  ['] describe-location-18 self~ be-description-xt
  s" puente de piedra" self~ ms-name!
  self~ be-indoor-location
  0 0 location-19~ location-17~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. puente, arco en escenario 18

: describe-location-19  ( -- )
  sight case
  my-location of
    ^the-water-current$ comma+
    s" que discurre" s?&
    s" de norte a este," s& (it)-blocks$ s&
    s" el paso, excepto al oeste." s&
    s{ s" Al" s" Del" s" Hacia el" s" Proveniente del" s" Procedente del" }s&
    s" fondo" s& s{ s" se oye" s" se escucha" s" puede oírse" }s&
    s" un gran estruendo." s&
      \ XXX TODO -- hacer variaciones de todo este texto
    paragraph
    endof
  north~ of
    ^water-from-there$ period+ paragraph
    endof
  east~ of
    water-that-way$ paragraph
    endof
  west~ of
    s" Se puede" to-go-back$ s&
    toward-the(m)$ s& s" arco de piedra" s&
    in-that-direction$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-19~ :init  ( -- )
  ['] describe-location-19 self~ be-description-xt
  s" recodo arenoso del canal" self~ ms-name!
  self~ be-indoor-location
  0 0 0 location-18~ 0 0 0 0 self~ init-location  ;

: describe-location-20  ( -- )
  sight case
  my-location of
    north~ south~ east~ 3 exits-cave-description paragraph
    endof
  north~ of
    cave-exit-description$ paragraph
    endof
  south~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

: can-i-enter-location-20?  ( -- f )
  location-17~ am-i-there? no-torch? and
  dup 0= swap ?? dark-cave  ;

location-20~ :init  ( -- )
  ['] describe-location-20 self~ be-description-xt
  ['] can-i-enter-location-20? self~ be-can-i-enter-location-xt
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-17~ location-22~ location-25~ 0 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- aplicar el filtro de la antorcha a todos los
  \ escenarios afectados, quizá en una capa superior
  \ sight no-torch? 0= abs *  case

: describe-location-21  ( -- )
  sight case
  my-location of
    east~ west~ south~ 3 exits-cave-description paragraph
    endof
  south~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-21~ :init  ( -- )
  ['] describe-location-21 self~ be-description-xt
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  0 location-27~ location-23~ location-20~ 0 0 0 0 self~ init-location  ;

: describe-location-22  ( -- )
  sight case
  my-location of
    south~ east~ west~ 3 exits-cave-description paragraph
    endof
  south~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-22~ :init  ( -- )
  ['] describe-location-22 self~ be-description-xt
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  0 location-24~ location-27~ location-22~ 0 0 0 0 self~ init-location  ;

: describe-location-23  ( -- )
  sight case
  my-location of
    west~ south~ 2 exits-cave-description paragraph
    endof
  south~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-23~ :init  ( -- )
  ['] describe-location-23 self~ be-description-xt
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  0 location-25~ 0 location-21~ 0 0 0 0 self~ init-location  ;

: describe-location-24  ( -- )
  sight case
  my-location of
    east~ north~ 2 exits-cave-description paragraph
    endof
  north~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-24~ :init  ( -- )
  ['] describe-location-24 self~ be-description-xt
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-22~ 0 location-26~ 0 0 0 0 0 self~ init-location  ;

: describe-location-25  ( -- )
  sight case
  my-location of
    north~ south~ east~ west~ 4 exits-cave-description paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-25~ :init  ( -- )
  ['] describe-location-25 self~ be-description-xt
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-22~ location-28~ location-23~ location-21~ 0 0 0 0 self~ init-location  ;

: describe-location-26  ( -- )
  sight case
  my-location of
    north~ east~ west~ 3 exits-cave-description paragraph
    endof
  north~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-26~ :init  ( -- )
  ['] describe-location-26 self~ be-description-xt
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-26~ 0 location-20~ location-27~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. pasaje/camino/senda tramo/cueva (en todos
  \ los tramos)

: describe-location-27  ( -- )
  sight case
  my-location of
    north~ east~ west~ 3 exits-cave-description paragraph
    endof
  north~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-27~ :init  ( -- )
  ['] describe-location-27 self~ be-description-xt
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-27~ 0 0 location-25~ 0 0 0 0 self~ init-location  ;

: describe-location-28  ( -- )
  sight case
  my-location of
    location-28~ ^full-name s" se extiende de norte a este." s&
    leader~ conversations?
    if  s" Hace de albergue para los refugiados."
    else  s" Está llen" location-28~ gender-ending+ s" de gente." s&
    then  s&
    flags~ is-known?
    if
      s" Hay" s&
      s{  s" una bandera de cada bando"
          s" banderas de" s{ s" ambos" s" los dos" }s& s" bandos" s&
          s{ s" dos banderas: una" s" una bandera" }s
              s{  s" britana y otra sajona" s" sajona y otra britana" }s&
      }s& period+
    else
      s" Hay" s& s{ s" dos" s" unas" }s& s" banderas." s&
    then
    paragraph
    endof
  north~ of
    location-28~ has-east-exit?
    if    s" Es por donde viniste."
    else  s" Es la única salida."
    then  paragraph
    endof
  east~ of
    ^the-refugees$
    location-28~ has-east-exit?
    if    they-let-you-pass$ s&
    else  they-don't-let-you-pass$ s&
    then  period+ paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-28  ( -- )
  location-28~ no-exit e-->  \ Cerrar la salida hacia el este
  recent-talks-to-the-leader off
  refugees~ be-here
  the-refugees-surround-you$ narrate
  the-leader-looks-at-you$ narrate  ;

location-28~ :init  ( -- )
  ['] describe-location-28 self~ be-description-xt
  ['] after-describing-location-28 self~ be-after-describing-location-xt
  s" amplia estancia" self~ fs-name!
  self~ be-indoor-location
  location-26~ 0 0 0 0 0 0 0 self~ init-location  ;
  \ XXX TODO -- crear ente. estancia(para todos),albergue y refugio
  \ (tras hablar con anciano) en escenario 28

: describe-location-29  ( -- )
  sight case
  my-location of
    s" Cual escalera de caracol gigante,"
    goes-down-into-the-deep$ comma+ s&
    s" dejando a los refugiados al oeste." s&
    paragraph
    endof
  west~ of
    over-there$ s" están los refugiados." s&
    paragraph
    endof
  down~ of
    s" La espiral" goes-down-into-the-deep$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-29  ( -- )
  refugees~ be-here  ;
  \ Para que sean visibles en la distancia

location-29~ :init  ( -- )
  ['] describe-location-29 self~ be-description-xt
  ['] after-describing-location-29 self~ be-after-describing-location-xt
  s" espiral" self~ fs-name!
  self~ be-indoor-location
  0 0 0 location-28~ 0 location-30~ 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. escalera/espiral, refugiados en escenario
  \ 29

: describe-location-30  ( -- )
  sight case
  my-location of
    s" Se eleva en la penumbra."
    s" La" s& cave$ s& gets-narrower(f)$ s&
    s" ahora como para una sola persona, hacia el este." s&
    paragraph
    endof
  east~ of
    s" La" cave$ s& gets-narrower(f)$ s& period+
    paragraph
    endof
  up~ of
    s" La" cave$ s& s" se eleva en la penumbra." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-30~ :init  ( -- )
  ['] describe-location-30 self~ be-description-xt
  s" inicio de la espiral" self~ ms-name!
  self~ be-indoor-location
  0 0 location-31~ 0 location-29~ 0 0 0 self~ init-location  ;

: describe-location-31  ( -- )
  sight case
  my-location of
    s" En este pasaje grandes rocas se encuentran entre las columnas de un arco de medio punto."
    paragraph
    endof
  north~ of
    s" Las rocas" location-31~ has-north-exit?
    if  (rocks)-on-the-floor$
    else  (they)-block$ the-pass$ s&
    then  s& period+ paragraph
    endof
  west~ of
    ^that-way$ s" se encuentra el inicio de la espiral." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-31  ( -- )
  \ XXX TODO -- mover a la descripción
  location-31~ has-north-exit? if
    s" Las rocas yacen desmoronadas a lo largo del"
    pass-way$ s& period+
  else
    s" Las rocas" (they)-block$ s& s" el paso." s&
  then  narrate  ;

location-31~ :init  ( -- )
  ['] describe-location-31 self~ be-description-xt
  ['] after-describing-location-31 self~ be-after-describing-location-xt
  s" puerta norte" self~ fs-name!
  self~ be-indoor-location
  0 0 0 location-30~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. arco, columnas, hueco/s(entre rocas) en
  \ escenario 31

: describe-location-32  ( -- )
  sight case
  my-location of
    s" El camino ahora no excede de dos palmos de cornisa sobre un abismo insondable."
    s" El soporte de roca gira en forma de «U» de oeste a sur." s&
    paragraph
    endof
  south~ of
    ^the-path$ s" gira" s& that-way$ s& period+
    paragraph
    endof
  west~ of
    ^the-path$ s" gira" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-32~ :init  ( -- )
  ['] describe-location-32 self~ be-description-xt
  s" precipicio" self~ ms-name!
  self~ be-indoor-location
  0 location-33~ 0 location-31~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. precipicio, abismo, cornisa, camino,
  \ roca/s en escenario 32

: describe-location-33  ( -- )
  sight case
  my-location of
    s" El paso se va haciendo menos estrecho a medida que se avanza hacia el sur, para entonces comenzar hacia el este."
    paragraph
    endof
  north~ of
    ^the-path$ s" se estrecha" s& that-way$ s& period+
    paragraph
    endof
  south~ of
    ^the-path$ gets-wider$ s& that-way$ s&
    s" y entonces gira hacia el este." s&
    paragraph
    endof
  east~ of
    ^the-path$ gets-wider$ s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-33~ :init  ( -- )
  ['] describe-location-33 self~ be-description-xt
  s" pasaje de salida" self~ ms-name!
  self~ be-indoor-location
  location-32~ 0 location-34~ 0 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. camino/paso/sendero en escenario 33

: describe-location-34  ( -- )
  sight case
  my-location of
    s" El paso" gets-wider$ s& s" de oeste a norte," s&
    s" y guijarros mojados y mohosos tachonan el suelo de roca." s&
    paragraph
    endof
  north~ of
    ^the-path$ gets-wider$ s& that-way$ s& period+
    paragraph
    endof
  west~ of
    ^the-path$ s" se estrecha" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-34~ :init  ( -- )
  ['] describe-location-34 self~ be-description-xt
  s" pasaje de gravilla" self~ ms-name!
  self~ be-indoor-location
  location-35~ 0 0 location-33~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. camino/paso/sendero, guijarros, moho,
  \ roca, suelo, gravillla en escenario 34


: describe-location-35  ( -- )
  sight case
  my-location of
    s" Un puente" s{ s" se tiende" s" cruza" }s& s" de norte a sur sobre el curso del agua." s&
    s" Unas resbaladizas escaleras" s& (they)-go-down$ s& s" hacia el oeste." s&
    paragraph
    endof
  north~ of
    bridge-that-way$ paragraph
    endof
  south~ of
    bridge-that-way$ paragraph
    endof
  west~ of
    stairway-to-river$ paragraph
    endof
  down~ of
    stairway-to-river$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-35~ :init  ( -- )
  ['] describe-location-35 self~ be-description-xt
  s" puente sobre el acueducto" self~ ms-name!
  self~ be-indoor-location
  location-40~ location-34~ 0 location-36~ 0 location-36~ 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. escaleras, puente, río/curso/agua en
  \ escenario 35

: describe-location-36  ( -- )
  sight case
  my-location of
    s" Una" s{ s" ruidosa" s" estruendosa" s" ensordecedora" }s&
    s" corriente" s& goes-down$ s&
    s{ s" con" s" siguiendo" }s& s" el" s& pass-way$ s&
    s" elevado desde el oeste, y forma un meandro arenoso." s&
    s" Unas escaleras" s& (they)-go-up$ s& toward-the(m)$ s& s" este." s&
    paragraph
    endof
  east~ of
    stairway-that-way$ paragraph
    endof
  west~ of
    ^water-from-there$ period+ paragraph
    endof
  up~ of
    stairway-that-way$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-36~ :init  ( -- )
  ['] describe-location-36 self~ be-description-xt
  s" remanso" self~ ms-name!
  self~ be-indoor-location
  0 0 location-35~ location-37~ location-35~ 0 0 0 self~ init-location  ;

: describe-location-37  ( -- )
  sight case
  my-location of
    s" El agua" goes-down$ s& s" por un canal" s?&
    from-the(m)$ s& s" oeste con" s&
    s{ s" renovadas fuerzas" s" renovada energía" s" renovado ímpetu" }s& comma+
    s" dejando" s& s{
    s" a un lado" a-high-narrow-pass-way$ s&
    a-high-narrow-pass-way$ s{ s" lateral" s" a un lado" }s&
    }s& s" que" s& lets-you$ s& to-keep-going$ s&
    toward-the(m)$ s" este" s&
    toward-the(m)$ s" oeste" s& rnd2swap s" o" s& 2swap s& s&
    period+ paragraph
    endof
  east~ of
    ^the-pass-way$ s" elevado" s?& lets-you$ s& to-keep-going$ s& that-way$ s& period+
    paragraph
    endof
  west~ of
    water-from-there$
    the-pass-way$ s" elevado" s?& lets-you$ s& to-keep-going$ s& that-way$ s&
    both s" también" s& ^uppercase period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-37~ :init  ( -- )
  ['] describe-location-37 self~ be-description-xt
  s" canal de agua" self~ ms-name!
  self~ be-indoor-location
  0 0 location-36~ location-38~ 0 0 0 0 self~ init-location  ;

: describe-location-38  ( -- )
  sight case
  my-location of
    s" Cae el agua hacia el este,"
    s{ s" descendiendo" s" bajando" }s&
    s{ s" con mucha fuerza" s" con gran fuerza" s" fuertemente" }s&
    s{ s" en dirección al" s" hacia el" }s& s" canal," s&
    s{ s" no sin antes" s" tras" s" después de" }s&
    s{ s" embalsarse" s" haberse embalsado" }s&
    s" en un lago" s&
    s{ s" no muy" s" no demasiado" s" poco" }s& s" profundo." s&
    paragraph
    endof
  east~ of
    water-that-way$ paragraph
    endof
  west~ of
    ^water-from-there$
    s" , de" s+ waterfall~ full-name s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-38~ :init  ( -- )
  ['] describe-location-38 self~ be-description-xt
  ['] lake-is-here self~ be-after-describing-location-xt
  s" gran cascada" self~ fs-name!
  self~ be-indoor-location
  0 0 location-37~ location-39~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- el artículo de «cascada» debe depender también de si
  \ se ha visitado el escenario 39 o este mismo 38

: describe-location-39  ( -- )
  sight case
  my-location of
    s" Musgoso y rocoso, con la cortina de agua"
    s{ s" tras de ti," s" a tu espalda," }s&
    s{ s" el nivel" s" la altura" }s& s" del agua ha" s&
    s{ s" subido" s" crecido" }s&
    s{ s" un poco" s" algo" }s& s" en este" s&
    s{ s" curioso" s" extraño" }s& s" hueco." s&
    paragraph
    endof
  east~ of
    s" Es la única salida." paragraph
      \ XXX TODO -- variar
    endof
  uninteresting-direction
  endcase  ;

location-39~ :init  ( -- )
  ['] describe-location-39 self~ be-description-xt
  s" interior de la cascada" self~ ms-name!
  self~ be-indoor-location
  0 0 location-38~ 0 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. musgo, cortina, agua, hueco en escenario
  \ 39

: describe-location-40  ( -- )
  sight case
  my-location of
    s" Una gran explanada enlosetada contempla un bello panorama de estalactitas."
    s" Unos casi imperceptibles escalones conducen al este." s&
    paragraph
    endof
  south~ of
    ^that-way$ s" se va" s& toward-the(m)$ s& s" puente." s&
    paragraph
    endof
  east~ of
    s" Los escalones" (they)-lead$ s& that-way$ s& period+
    paragraph
    endof
  up~ of
    s{ s" Sobre" s" Por encima de" }s
    s{ s" ti" s" tu cabeza" }s& s" se" s&
    s{ s" exhibe" s" extiende" s" disfruta" }s&
    s" un" s& beautiful(m)$ s&
    s{ s" panorama" s" paisaje" }s s& s" de estalactitas." s&
    paragraph
    endof
  down~ of
    s" Es una" s{ s" gran" s" buena" }s s& s" explanada enlosetada." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-40~ :init  ( -- )
  ['] describe-location-40 self~ be-description-xt
  s" explanada" self~ fs-name!
  self~ be-indoor-location
  0 location-35~ location-41~ 0 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. losas y losetas, estalactitas, panorama,
  \ escalones en escenario 40

: describe-location-41  ( -- )
  sight case
  my-location of
    s" El ídolo parece un centinela siniestro de una gran roca que se encuentra al sur."
    s" Se puede" s& to-go-back$ s& toward$ s& s" la explanada hacia el oeste." s&
    paragraph
    endof
  south~ of
    s" Hay una" s" roca" s" enorme" rnd2swap s& s&
    that-way$ s& period+
    paragraph
    endof
  west~ of
    s" Se puede volver" toward$ s& s" la explanada" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-41~ :init  ( -- )
  ['] describe-location-41 self~ be-description-xt
  self~ be-indoor-location
  s" ídolo" self~ ms-name!
  0 0 0 location-40~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. roca, centinela en escenario 41

: describe-location-42  ( -- )
  sight case
  my-location of
    s" Como un pasillo que corteja el canal de agua, a su lado, baja de norte a sur."
    paragraph
    endof
  north~ of
    ^the-pass-way$ goes-up$ s& that-way$ s&
    s" , de donde" s{ s" corre" s" procede" s" viene" s" proviene" }s& s" el agua." s& s+
    paragraph
    endof
  south~ of
    ^the-pass-way$ goes-down$ s& that-way$ s&
    s" , siguiendo el canal de agua," s+
    s" hacia un lugar en que" s&
    s{ s" se aprecia" s" puede apreciarse" s" se distingue" }s&
    s" un aumento de luz." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-42~ :init  ( -- )
  ['] describe-location-42 self~ be-description-xt
  s" pasaje estrecho" self~ ms-name!
  self~ be-indoor-location
  location-41~ location-43~ 0 0 0 0 0 0 self~ init-location  ;

: describe-location-43  ( -- )
  sight case
  my-location of
    ^the-pass-way$ s" sigue de norte a sur." s&
    paragraph
    endof
  north~ of
    ^the-pass-way$ s" continúa" s& that-way$ s& period+
    paragraph
    endof
  south~ of
    snake~ is-here?
    if  a-snake-blocks-the-way$
    else  ^the-pass-way$ s" continúa" s& that-way$ s&
    then  period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-43  ( -- )
  snake~ is-here? if
    a-snake-blocks-the-way$ period+
    narrate
  then  ;

location-43~ :init  ( -- )
  ['] describe-location-43 self~ be-description-xt
  ['] after-describing-location-43 self~ be-after-describing-location-xt
  s" pasaje de la serpiente" self~ ms-name!
  self~ be-indoor-location
  location-42~ 0 0 0 0 0 0 0 self~ init-location  ;

: describe-location-44  ( -- )
  sight case
  my-location of
    s" Unas escaleras" s{ s" dan" s" permiten el" }s& s{ s" paso" s" acceso" }s&
    s" a un" s& beautiful(m)$ s& s" lago interior, hacia el oeste." s&
    s" Al norte, un oscuro y"
    narrow(m)$ s& pass-way$ s& goes-up$ s& period+ s?&
    paragraph
    endof
  north~ of
    s" Un pasaje oscuro y" narrow(m)$ s& goes-up$ s& that-way$ s& period+
    paragraph
    endof
  west~ of
    s" Las escaleras" (they)-lead$ s& that-way$ s& s" , hacia el lago" s?+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-44~ :init  ( -- )
  ['] describe-location-44 self~ be-description-xt
  ['] lake-is-here self~ be-after-describing-location-xt
  s" lago interior" self~ ms-name!
  self~ be-indoor-location
  location-43~ 0 0 location-45~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. lago, escaleras, pasaje, lago en escenario
  \ 44

: describe-location-45  ( -- )
  sight case
  my-location of
    ^narrow(mp)$ pass-ways$ s&
    s" permiten ir al oeste, al este y al sur." s&
    paragraph
    endof
  south~ of
    ^a-narrow-pass-way$ s" permite ir" s& that-way$ s&
    s" , de donde" s+ s{ s" proviene" s" procede" }s&
    s{ s" una gran" s" mucha" }s& s" luminosidad." s&
    paragraph
    endof
  west~ of
    ^a-narrow-pass-way$ leads$ s& that-way$ s& period+
    paragraph
    endof
  east~ of
    ^a-narrow-pass-way$ leads$ s& that-way$ s& period+
    s" , de donde" s{ s" proviene" s" procede" }s&
    s{ s" algo de" s" una poca" s" un poco de" }s&
    s{ s" claridad" s" luz" }s& period+ s+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-45~ :init  ( -- )
  ['] describe-location-45 self~ be-description-xt
  s" cruce de pasajes" self~ ms-name!
  self~ be-indoor-location
  0 location-47~ location-44~ location-46~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. pasaje/camino/paso/senda en escenario 45

: describe-location-46  ( -- )
  sight case
  my-location of
    s" Un catre, algunas velas y una mesa es todo lo que"
    s{ s" tiene" s" posee" }s s" Ambrosio" rnd2swap s& s&
    period+  paragraph
    endof
  east~ of
    s" La salida"
    s{ s" de la casa" s" del hogar" }s s" de Ambrosio" s& s?&
    s" está" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-46~ :init  ( -- )
  ['] describe-location-46 self~ be-description-xt
  s" hogar de Ambrosio" self~ ms-name!
  self~ be-indoor-location
  0 0 location-45~ 0 0 0 0 0 self~ init-location  ;

: describe-location-47  ( -- )
  sight case
  my-location of
    s" Por el oeste,"
    door~ full-name s& door~ «open»|«closed» s& comma+
    door~ is-open? if  \ La puerta está abierta
      s" por la cual entra la luz que ilumina la estancia," s&
      s" permite salir de la cueva." s&
    else  \ La puerta está cerrada
      s" al otro lado de la cual se adivina la luz diurna," s&
      door~ is-known?
      if    s" impide" s&
      else  s" parece ser" s&
      then  s" la salida de la cueva." s&
    then
    paragraph
    endof
  north~ of
    s" Hay salida" that-way$ s& period+ paragraph
      \ XXX TODO -- variar el texto
    endof
  west~ of
    door~ is-open?
    if    s" La luz diurna entra por la puerta."
    else  s" Se adivina la luz diurna al otro lado de la puerta."
    then  paragraph
      \ XXX TODO -- variar el texto
    endof
  uninteresting-direction
  endcase  ;

location-47~ :init  ( -- )
  ['] describe-location-47 self~ be-description-xt
  ['] door-is-here self~ be-after-describing-location-xt
  s" salida de la cueva" self~ fs-name!
  self~ be-indoor-location
  location-45~ 0 0 0 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- descripción inacabada de escenario 47

: when-the-door$  ( -- ca len )
  s" cuando" s{ s" la" s" su" }s& s" puerta" s&  ;

: like-now$+  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  s" , como ahora" s?+  ;

: describe-location-48  ( -- )
  sight case
  my-location of
    s{ s" Apenas si" s" Casi no" }s
    s{ s" se puede" s" es posible" }s&
    s" reconocer la entrada de la cueva, al este." s&
    ^the-path$ s& s{ s" parte" s" sale" }s&
    s" del bosque hacia el oeste." s&
    paragraph
    endof
  east~ of
    s" La entrada de la cueva" s{
    s" está" s" bien" s?& s{ s" camuflada" s" escondida" }s&
    s" apenas se ve" s" casi no se ve" s" pasa casi desapercibida"
    }s& comma+
    door~ is-open? if
      even$ s& when-the-door$ s&
      s{ s" está abierta" s" no está cerrada" }s& like-now$+
    else
      s{ s" especialmente" s" sobre todo" }s& when-the-door$ s&
      s{ s" no está abierta" s" está cerrada" }s& like-now$+
    then  period+ paragraph
    endof
  west~ of
    ^the-path$ s{ s" parte" s" sale" }s& s" del bosque" s& in-that-direction$ s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-48~ :init  ( -- )
  ['] describe-location-48 self~ be-description-xt
  ['] door-is-here self~ be-after-describing-location-xt
  s" bosque a la entrada" self~ ms-name!
  0 0 location-47~ location-49~ 0 0 0 0 self~ init-location  ;

  \ XXX TODO -- crear ente. cueva en escenario 48

: describe-location-49  ( -- )
  sight case
  my-location of
    ^the-path$ s" recorre" s& s" toda" s?&
    s" esta" s& s{ s" parte" s" zona" }s&
    s" del bosque de este a oeste." s&
    paragraph
    endof
  east~ of
    ^the-path$ leads$ s&
    s" al bosque a la entrada de la cueva." s&
    paragraph
    endof
  west~ of
    ^the-path$ s" continúa" s& in-that-direction$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-49~ :init  ( -- )
  ['] describe-location-49 self~ be-description-xt
  s" sendero del bosque" self~ ms-name!
  0 0 location-48~ location-50~ 0 0 0 0 self~ init-location  ;

: describe-location-50  ( -- )
  sight case
  my-location of
    s" El camino norte" s{ s" que sale" s" que parte" s" procedente" }s&
    s" de Westmorland se" s{ s" interna" s" adentra" }s& s" en el bosque," s&
    s" aunque en tu estado no puedes ir." s&
    paragraph
    endof
  south~ of
    s{ s" ¡Westmorland!" s" Westmorland..." }s
    paragraph
    endof
  east~ of
    ^can-see$ s" el sendero del bosque." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-50~ :init  ( -- )
  ['] describe-location-50 self~ be-description-xt
  s" camino norte" self~ ms-name!
  0 location-51~ location-49~ 0 0 0 0 0 self~ init-location  ;

: describe-location-51  ( -- )
  sight case
  my-location of
    ^the-village$ s" bulle de actividad con el mercado en el centro de la plaza," s&
    s" donde se encuentra el castillo." s&
    paragraph
    endof
  north~ of
    s" El camino norte" of-the-village$ s& leads$ s& s" hasta el bosque." s&
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-51~ :init  ( -- )
  ['] describe-location-51 self~ be-description-xt
  s" Westmorland" self~ fs-name!
  self~ have-no-article
  location-50~ 0 0 0 0 0 0 0 self~ init-location  ;
  \ XXX TODO -- crear ente. mercado, plaza, villa, pueblo, castillo

\ ----------------------------------------------
\ Entes globales

: describe-cave  ( -- )
  s" La cueva es húmeda y sombría."
  paragraph  ;
  \ XXX TODO -- mejorar

cave~ :init  ( -- )
  ['] describe-cave self~ be-description-xt
  s" cueva" self~ fs-name!  ;
  \ self~ be-global-indoor \ XXX OLD

: describe-ceiling  ( -- )
  s" El techo es muy bonito."
  paragraph  ;
  \ XXX TODO

ceiling~ :init  ( -- )
  ['] describe-ceiling self~ be-description-xt
  s" techo" self~ ms-name!
  self~ be-global-indoor  ;

: describe-clouds  ( -- )
  s" Los estratocúmulos que traen la nieve y que cuelgan sobre la Tierra"
  s" en la estación del frío se han alejado por el momento. " s&
    \ XXX TMP
  2 random if  paragraph  else  2drop sky~ describe  then  ;
    \ XXX TODO -- comprobar

clouds~ :init  ( -- )
  ['] describe-clouds self~ be-description-xt
  s" nubes" self~ fp-name!
  self~ be-global-outdoor  ;
  \ XXX TODO:
  \ Distinguir no solo interiores, sino escenarios en
  \ que se puede vislumbrar el exterior.

: describe-floor  ( -- )
  am-i-outdoor?
  if    s" [El suelo fuera es muy bonito.]" paragraph
  else  s" [El suelo dentro es muy bonito.]" paragraph  then  ;
  \ XXX TMP
  \ XXX TODO

floor~ :init  ( -- )
  ['] describe-floor self~ be-description-xt
  s" suelo" self~ ms-name!
  self~ be-global-indoor
  self~ be-global-outdoor  ;

: describe-sky  ( -- )
  s" [El cielo es muy bonito.]"
  paragraph  ;
  \ XXX TODO
  \ XXX TMP

sky~ :init  ( -- )
  ['] describe-sky self~ be-description-xt
  s" cielo" self~ ms-name!
  self~ be-global-outdoor  ;
  \ XXX TODO
  \ XXX TMP

: describe-wall  ( -- )
  s" [La pared es muy bonita.]"
  paragraph  ;
  \ XXX TODO
  \ XXX TMP

wall~ :init  ( -- )
  ['] describe-wall self~ be-description-xt
  s" pared" self~ ms-name!
  self~ be-global-indoor  ;

\ ----------------------------------------------
\ Entes virtuales

defer describe-exits  ( -- )

exits~ :init  ( -- )
  ['] describe-exits self~ be-description-xt
  s" salida" self~ fs-name!
  self~ be-global-outdoor
  self~ be-global-indoor  ;

defer describe-inventory  ( -- )

inventory~ :init  ( -- )
  ['] describe-inventory self~ be-description-xt  ;

: describe-enemy  ( -- )
  battle# @
  if    s" [Enemigo en batalla.]"
  else  s" [Enemigo en paz.]"
  then  paragraph  ;
  \ XXX TMP
  \ XXX TODO -- inconcluso

enemy~ :init  ( -- )
  ['] describe-enemy self~ be-description-xt
  s" enemigos" self~ mp-name!
  self~ be-human
  self~ be-decoration  ;
  \ XXX TODO -- inconcluso

\ ----------------------------------------------
\ Entes dirección

\ Los entes dirección guardan en su campo `~direction`
\ el desplazamiento correspodiente al campo de
\ dirección que representan
\ Esto sirve para reconocerlos como tales entes dirección
\ (pues todos los valores posibles son diferentes de cero)
\ y para hacer los cálculos en las acciones de movimiento.

north~ :init  ( -- )
  \ ['] describe-north self~ be-description-xt
  s" norte" self~ ms-name!
  self~ have-definite-article
  north-exit> self~ ~direction !  ;

south~ :init  ( -- )
  \ ['] describe-south self~ be-description-xt
  s" sur" self~ ms-name!
  self~ have-definite-article
  south-exit> self~ ~direction !  ;

east~ :init  ( -- )
  \ ['] describe-east self~ be-description-xt
  s" este" self~ ms-name!
  self~ have-definite-article
  east-exit> self~ ~direction !  ;

west~ :init  ( -- )
  \ ['] describe-west self~ be-description-xt
  s" oeste" self~ name!
  self~ have-definite-article
  west-exit> self~ ~direction !  ;

: describe-up  ( -- )
  am-i-outdoor?
  if  sky~ describe
  else  ceiling~ describe
  then  ;

up~ :init  ( -- )
  ['] describe-up self~ be-description-xt
  s" arriba" self~ name!
  self~ have-no-article
  up-exit> self~ ~direction !  ;

: describe-down  ( -- )
  am-i-outdoor?
  if    s" [El suelo exterior es muy bonito.]" paragraph
  else  s" [El suelo interior es muy bonito.]" paragraph
  then  ;
  \ XXX TMP
  \ XXX TODO

down~ :init  ( -- )
  ['] describe-down self~ be-description-xt
  s" abajo" self~ name!
  self~ have-no-article
  down-exit> self~ ~direction !  ;

out~ :init  ( -- )
  \ ['] describe-out self~ be-description-xt
  s" afuera" self~ name!
  self~ have-no-article
  out-exit> self~ ~direction !  ;

in~ :init  ( -- )
  \ ['] describe-in self~ be-description-xt
  s" adentro" self~ name!
  self~ have-no-article
  in-exit> self~ ~direction !  ;

\ vim:filetype=gforth:fileencoding=utf-8

