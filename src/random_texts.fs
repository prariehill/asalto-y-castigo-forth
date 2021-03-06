\ random_texts.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201711072220

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/choose.fs                  \ `choose`
require galope/fifty-percent-nullify.fs   \ `50%nullify`
require galope/question-question.fs       \ `??`
require galope/s-curly-bracket.fs         \ `s{`
require galope/replaced.fs                \ `replaced`
require galope/sconstant.fs               \ `sconstant`
require galope/txt-plus.fs                \ `txt+`

set-current

\ ==============================================================

\ Casi todas las palabras de esta sección devuelven una cadena
\ calculada al azar. Las restantes palabras son auxiliares.

\ Por convención, en el programa las palabras que devuelven
\ una cadena sin recibir parámetros en la pila tienen el signo
\ «$» al final de su nombre.  También por tanto las constantes
\ de cadena creadas con `sconstant`.

: at-least$ ( ca len -- )
  s{ s" al" s" por lo" }s s" menos" txt+ ;

: that-(at-least)$ ( ca len -- )
  s" que" at-least$ 50%nullify txt+ ;

: that(m)$ ( -- ca len )
  s{ s" que" s" cual" }s ;

: the-that(m)$ ( -- ca len )
  s{ s" que" s" el cual" }s ;

: this|the(f)$ ( -- ca len )
  s{ s" esta" s" la" }s ;

: this|the(m)$ ( -- ca len )
  s{ s" este" s" el" }s ;
  \ XXX TODO -- no usado

: your|the(f)$ ( -- ca len )
  s{ s" tu" s" la" }s ;

: old-man$ ( -- ca len )
  s{ s" hombre" s" viejo" s" anciano" }s ;
  \ Devuelve una forma de llamar al líder de los refugiados.

: with-him$ ( -- ca len )
  s{ null$ s" consigo" s" encima" }s ;
  \ Devuelve una variante de «consigo» o una cadena vacía.

: with-you$ ( -- ca len )
  s" contigo" 50%nullify ;
  \ Devuelve «contigo» o una cadena vacía.

: carries$ ( -- ca len )
  s{ s" tiene" s" lleva" }s ;

: you-carry$ ( -- ca len )
  s{ s" tienes" s" llevas" }s ;

: ^you-carry$ ( -- ca len )
  you-carry$ xcapitalized ;
  \ Devuelve una variante de «Llevas» (con la primera mayúscula).

: now$ ( -- ca len )
  s{ null$ s" ahora" s" en este momento" s" en estos momentos" }s ;
  \ Devuelve una variante de «ahora» o una cadena vacía.

: now-or-null$ ( -- ca len ) now$ 50%nullify ;
  \ Devuelve el resultado de `now$` o una cadena vacía.  Sirve como
  \ variante de `now$` con mayor probabilidad devolver una cadena
  \ vacía.

: here$ ( -- ca len )
  s{ s" por aquí" s" por este lugar" s" en este lugar" s" aquí" }s ;
  \ Devuelve una variante de «aquí».

: here-or-null$ ( -- ca len ) here$ 50%nullify ;
  \ Devuelve el resultado de `here$` o una cadena vacía.

: now-or-here-or-null$ ( -- ca len ) s{ now$ here-or-null$ }s ;
  \ Devuelve el resultado de `now$` o de `here-or-null$`, o una cadena
  \ vacía.

: only$ ( -- ca len )
  s{ s" tan solo" s" solo" s" solamente" s" únicamente" }s ;
  \ Devuelve una variante de «solamente».

: ^only$ ( -- ca len )
  s{ s" Tan solo" s" Solo" s" Solamente" s" Únicamente" }s ;
  \ Devuelve una variante de «Solamente» (con la primera mayúscula).
  \ Nota: no se puede calcular este texto a partir de la versión en minúsculas, porque el cambio entre minúsculas y mayúsculas no funciona con caracteres codificados en UTF-8 de más de un octeto.

: only-or-null$ ( -- ca len ) only$ 50%nullify ;
  \ Devuelve el resultado de `only$` o una cadena vacía.

: ^only-or-null$ ( -- ca len ) ^only$ 50%nullify ;
  \ Devuelve el resultado de `^only$` o una cadena vacía.

: again$ ( -- ca len )
  s{ s" de nuevo" s" otra vez" s" otra vez más" s" una vez más" }s ;

: ^again$ ( -- ca len )
  again$ xcapitalized ;

: again?$ ( -- ca len )
  again$ s" ?" s+ ;

: still$ ( -- ca len )
  s{ s" aún" s" todavía" }s ;

: even$ ( -- ca len )
  s{ s" aun" s" incluso" }s ;

: toward$ ( -- ca len )
  s{ s" a" s" hacia" }s ;

: toward-the(f)$ ( -- ca len )
  toward$ s" la" txt+ ;

: toward-the(m)$ ( -- ca len )
  s{ s" al" s" hacia el" }s ;

: ^toward-the(m)$ ( -- ca len )
  toward-the(m)$ xcapitalized ;

: from-the(m)$ ( -- ca len )
  s{ s" desde el" s" procedente" 50%nullify s" del" txt+ }s ;

: to-go-back$ ( -- ca len )
  s{ s" volver" s" regresar" }s ;

: remains$ ( -- ca len )
  s{ s" resta" s" queda" }s ;

: possible1$ ( -- ca len )
  s" posible" 50%nullify ;
  \ Devuelve «posible» o una cadena vacía.

: possible2$ ( -- ca len )
  s" posibles" 50%nullify ;
  \ Devuelve «posibles» o una cadena vacía.

: all-your$ ( -- ca len )
  s{ s" todos tus" s" tus" }s ;
  \ Devuelve una variante de «todos tus».

: ^all-your$ ( -- ca len )
  all-your$ xcapitalized ;
  \ Devuelve una variante de «Todos tus» (con la primera mayúscula).

: soldiers$ ( -- ca len )
  s{ s" hombres" s" soldados" }s ;
  \ Devuelve una variante de «soldados».

: your-soldiers$ ( -- ca len )
  s" tus" soldiers$ txt+ ;
  \ Devuelve una variante de "tus hombres".

: ^your-soldiers$ ( -- ca len )
  your-soldiers$ xcapitalized ;
  \ Devuelve una variante de "Tus hombres".

: officers$ ( -- ca len )
  s{ s" oficiales" s" mandos" }s ;
  \ Devuelve una variante de «oficiales».

: the-enemies$ ( -- ca len )
  s{ s" los sajones"
  s{ s" las tropas" s" las huestes" }s
  s{ s" enemigas" s" sajonas" }s txt+ }s ;
  \ Devuelve una variante de «los enemigos».

: the-enemy$ ( -- ca len )
  s{ s" el enemigo"
  s{ s" la tropa" s" la hueste" }s
  s{ s" enemiga" s" sajona" }s txt+ }s ;
  \ Devuelve una variante de «el enemigo».

: (the-enemy|enemies) ( -- ca len f )
  2 random dup if  the-enemies$  else  the-enemy$  then  rot ;
  \ Devuelve una variante de «el/los enemigo/s», y un indicador del número.
  \ ca len = Cadena con el texto
  \ f = ¿El texto está en plural?

: the-enemy|enemies$ ( -- ca len )
  (the-enemy|enemies) drop ;
  \ Devuelve una variante de «el/los enemigo/s».

: «de-el»>«del» ( ca1 len1 -- ca1 len1 | ca2 len2 )
  s" del " s" de el " replaced ;
  \ Remplaza las apariciones de «de el» en una cadena por «del».

: of-the-enemy|enemies$ ( -- ca len )
  (the-enemy|enemies) >r
  s" de" 2swap txt+
  r> 0= ?? «de-el»>«del» ;
  \ Devuelve una variante de «del/de los enemigo/s».

: ^the-enemy|enemies ( -- ca len f )
  (the-enemy|enemies) >r  xcapitalized  r> ;
  \ Devuelve una variante de «El/Los enemigo/s», y un indicador del número.
  \ ca len = Cadena con el texto
  \ f = ¿El texto está en plural?

: of-your-ex-cloak$ ( -- ca len )
  s{ null$ s" que queda" s" que quedó" }s s" de" txt+
  s{ s" lo" s" la" }s txt+ s" que" txt+ s" antes" 50%nullify txt+
  s{ s" era" s" fue" s" fuera" }s txt+
  your|the(f)$ txt+ s{ s" negra" s" oscura" }s 50%nullify txt+
  s" capa" txt+ s" de lana" 50%nullify txt+ period+ ;
  \ Devuelve un texto común a las descripciones de los restos de la capa.

: but$ ( -- ca len )
  s{ s" pero" s" mas" }s ;

: ^but$ ( -- ca len )
  but$ xcapitalized ;

: though$ ( -- ca len )
  s{ s" si bien" but$ s" aunque" }s ;

: place$ ( -- ca len )
  s{ s" sitio" s" lugar" }s ;

: cave$ ( -- ca len )
  s{ s" cueva" s" caverna" s" gruta" }s ;

: home$ ( -- ca len )
  s{ s" hogar" s" casa" }s ;

: sire,$ ( -- ca len )
  s" Sire" s" Ulfius" 50%nullify txt+ comma+ ;

: my-name-is$ ( -- ca len )
  s{ s" Me llamo" s" Mi nombre es" }s ;

: very$ ( -- ca len )
  s{ s" muy" s" harto" s" asaz" }s ;

: very-or-null$ ( -- ca len ) very$ 50%nullify ;
  \ Devuelve el resultado de `very$` o una cadena vacía.

: the-path$ ( -- ca len )
  s{ s" el camino" s" la senda" s" el sendero" }s ;

: ^the-path$ ( -- ca len )
  the-path$ xcapitalized ;

: a-path$ ( -- ca len )
  s{ s" un camino" s" una senda" }s ;

: ^a-path$ ( -- ca len )
  a-path$ xcapitalized ;

: pass$ ( -- ca len )
  s{ s" paso" s" camino" }s ;

: the-pass$ ( -- ca len )
  s" el" pass$ txt+ ;

: pass-way$ ( -- ca len )
  s{ s" paso" s" pasaje" }s ;
  \ Devuelve una variante de «pasaje».

: a-pass-way$ ( -- ca len )
  s" un" pass-way$ txt+ ;
  \ Devuelve una variante de «un pasaje».

: ^a-pass-way$ ( -- ca len )
  a-pass-way$ xcapitalized ;
  \ Devuelve una variante de «Un pasaje» (con la primera mayúscula).

: the-pass-way$ ( -- ca len )
  s" el" pass-way$ txt+ ;
  \ Devuelve una variante de «el pasaje».

: ^the-pass-way$ ( -- ca len )
  the-pass-way$ xcapitalized ;
  \ Devuelve una variante de «El pasaje» (con la primera mayúscula).

: pass-ways$ ( -- ca len )
  pass-way$ s" s" s+ ;
  \ Devuelve una variante de «pasajes».

: ^pass-ways$ ( -- ca len )
  pass-ways$ xcapitalized ;
  \ Devuelve una variante de «Pasajes» (con la primera mayúscula).

: surrounds$ ( -- ca len )
  s{ s" rodea" s" circunvala" s" cerca" s" circuye" s" da un rodeo a" }s ;
  \ XXX TODO -- comprobar traducción

: leads$ ( -- ca len )
  s{ s" lleva" s" conduce" }s ;

: (they)-lead$ ( -- ca len )
  leads$ s" n" s+ ;

: can-see$ ( -- ca len )
  s{ s" ves" s" se ve" s" puedes ver" }s ;
  \ Devuelve una forma de decir «ves».

: ^can-see$ ( -- ca len )
  can-see$ xcapitalized ;
  \ Devuelve una forma de decir «ves», con la primera letra mayúscula.

: cannot-see$ ( -- ca len )
  s" no" can-see$ txt+ ;
  \ Devuelve una forma de decir «no ves».

: ^cannot-see$ ( -- ca len )
  cannot-see$ xcapitalized ;
  \ Devuelve una forma de decir «No ves».

: can-glimpse$ ( -- ca len )
  s{ s" vislumbras" s" se vislumbra" s" puedes vislumbrar"
  s" entrevés" s" se entrevé" s" puedes entrever"
  s" columbras" s" se columbra" s" puedes columbrar" }s ;

: ^can-glimpse$ ( -- ca len )
  can-glimpse$ xcapitalized ;

: in-half-darkness-you-glimpse$ ( -- ca len )
  s" En la" s{ s" semioscuridad," s" penumbra," }s txt+ 50%nullify dup
  if  can-glimpse$  else  ^can-glimpse$  then  txt+ ;
  \ Devuelve un texto usado en varias descripciones de las cuevas.

: you-glimpse-the-cave$ ( -- a u)
  in-half-darkness-you-glimpse$ s" la continuación de la cueva." txt+ ;
  \ Devuelve un texto usado en varias descripciones de las cuevas.
  \ XXX TODO -- distinguir la antorcha encendida

: rimarkable$ ( -- ca len )
  s{ s" destacable" s" que destacar"
  s" especial" s" de especial"
  s" de particular"
  s" peculiar" s" de peculiar"
  s" que llame la atención" }s ;
  \ Devuelve una variante de «destacable».

: has-nothing$ ( -- ca len )
  s" no tiene nada" ;

: is-normal$ ( -- ca len )
  has-nothing$ rimarkable$ txt+ ;
  \ Devuelve una variante de «no tiene nada especial».

: ^is-normal$ ( -- ca len )
  is-normal$ xcapitalized ;
  \ Devuelve una variante de «No tiene nada especial»
  \ (con la primera letra en mayúscula).

: over-there$ ( -- ca len )
  s{ s" allí" s" allá" }s ;

: goes-down-into-the-deep$ ( -- ca len )
  s{ s" desciende" toward$ txt+ s" se adentra en"
  s" conduce" toward$ txt+ s" baja" toward$ txt+ }s
  s" las profundidades" txt+ ;
  \ Devuelve una variante de «desciende a las profundidades».

: in-that-direction$ ( -- ca len )
  s{ s" en esa dirección" s{ s" por" s" hacia" }s over-there$ txt+ }s ;
  \ Devuelve una variante de «en esa dirección».

: ^in-that-direction$ ( -- ca len )
  in-that-direction$ xcapitalized ;
  \ Devuelve una variante de «En esa dirección».

: (uninteresting-direction-0)$ ( -- ca len )
  s{ s" Esa dirección" is-normal$ txt+
  ^in-that-direction$ s" no hay nada" txt+ rimarkable$ txt+
  ^in-that-direction$ cannot-see$ txt+ s" nada" txt+ rimarkable$ txt+
  }s period+ ;
  \ Devuelve primera variante de «En esa dirección no hay nada especial».

: (uninteresting-direction-1)$ ( -- ca len )
  s{
  ^is-normal$ s" esa dirección" txt+
  ^cannot-see$ s" nada" txt+ rimarkable$ txt+ in-that-direction$ txt+
  s" No hay nada" rimarkable$ txt+ in-that-direction$ txt+
  }s period+ ;
  \ Devuelve segunda variante de «En esa dirección no hay nada especial».

: uninteresting-direction$ ( -- ca len )
  ['] (uninteresting-direction-0)$
  ['] (uninteresting-direction-1)$
  2 choose execute ;
  \ Devuelve una variante de «En esa dirección no hay nada especial».

s" de Westmorland" sconstant of-westmorland$
: the-village$ ( -- ca len )
  s{ s" la villa" of-westmorland$ 50%nullify txt+
  s" Westmorland" }s ;

: ^the-village$ ( -- ca len )
  the-village$ xcapitalized ;

: of-the-village$ ( -- ca len )
  s" de" the-village$ txt+ ;

: (it)-blocks$ ( -- ca len )
  s{ s" impide" s" bloquea" }s ;

: (they)-block$ ( -- ca len )
  s{ s" impiden" s" bloquean" }s ;

: (rocks)-on-the-floor$ ( -- ca len )
  s" yacen desmoronadas" s" a lo largo del pasaje" 50%nullify txt+ ;
  \ Devuelve un texto sobre las rocas que ya han sido desmoronadas.

: (rocks)-clue$ ( -- ca len )
  s" Son" s{ s" muchas" s" muy" 50%nullify s" numerosas" txt+ }s txt+ comma+
  s" aunque no parecen demasiado pesadas y" txt+
  s{ s" pueden verse" s" se ven" s" hay" }s s" algunos huecos" txt+
  s" entre ellas" rnd2swap txt+ txt+ ;
  \ Devuelve una descripción de las rocas que sirve de pista.

: from-that-way$ ( -- u )
  s" de" s{ s" esa dirección" s" allí" s" ahí" s" allá" }s txt+ ;

: that-way$ ( -- ca len )
  s{ s" en esa dirección" s" por" s{ s" ahí" s" allí" s" allá" }s txt+ }s ;
  \ Devuelve una variante de «en esa dirección».

: ^that-way$ ( -- ca len )
  that-way$ xcapitalized ;
  \ Devuelve una variante de «En esa dirección»
  \ (con la primera letra mayúscula).

: gets-wider$ ( -- ca len )
  \ Devuelve una variante de «se ensancha».
  s{
  s" se" s{ s" ensancha" s" va ensanchando"
  s" va haciendo más ancho" s" hace más ancho"
  s" vuelve más ancho" s" va volviendo más ancho" }s txt+
  2dup 2dup 2dup \ Aumentar las probabilidades de la primera variante
  s{ s" ensánchase" s" hácese más ancho" s" vuélvese más ancho" }s
  }s ;

: (narrow)$ ( -- ca len )
  s{ s" estrech" s" angost" }s ;

: narrow(f)$ ( -- ca len )
  (narrow)$ s" a" s+ ;
  \ Devuelve una variante de «estrecha».

: narrow(m)$ ( -- ca len )
  (narrow)$ s" o" s+ ;
  \ Devuelve una variante de «estrecho».

: narrow(mp)$ ( -- ca len )
  narrow(m)$ s" s" s+ ;
  \ Devuelve una variante de «estrechos».

: ^narrow(mp)$ ( -- ca len )
  narrow(mp)$  xcapitalized ;
  \ Devuelve una variante de «Estrechos» (con la primera mayúscula).

: gets-narrower(f)$ ( -- ca len )
  s{
  s" se" s{ s" estrecha" s" va estrechando" }s txt+
  2dup \ Aumentar las probabilidades de la primera variante
  s" se" s{ s" va haciendo más" s" hace más"
  s" vuelve más" s" va volviendo más" }s txt+ narrow(f)$ txt+
  2dup \ Aumentar las probabilidades de la segunda variante
  s{ s" estréchase" s{ s" hácese" s" vuélvese" }s s" más" txt+ narrow(f)$ txt+ }s
  }s ;
  \ Devuelve una variante de «se hace más estrecha» (femenino).

: goes-up$ ( -- ca len )
  s{ s" sube" s" asciende" }s ;
  \ Devuelve una variante de «sube».

: (they)-go-up$ ( -- ca len )
  goes-up$ s" n" s+ ;
  \ Devuelve una variante de «suben».

: goes-down$ ( -- ca len )
  s{ s" baja" s" desciende" }s ;
  \ Devuelve una variante de «baja».

: (they)-go-down$ ( -- ca len )
  goes-down$ s" n" s+ ;
  \ Devuelve una variante de «bajan».

: almost-invisible(plural)$ ( -- ca len )
  s" casi" s{ s" imperceptibles" s" invisibles" s" desapercibidos" }s ;
  \ Devuelve una variante de «casi imperceptibles».
  \ XXX TODO -- confirmar significados

: ^a-narrow-pass-way$ ( -- ca len )
  s" Un" narrow(m)$ pass-way$ rnd2swap txt+ txt+ ;

: beautiful(m)$ ( -- ca len )
  s{ s" bonito" s" bello" s" hermoso" }s ;

: a-snake-blocks-the-way$ ( -- ca len )
  s" Una serpiente"
  s{ s" bloquea" s" está bloqueando" }s txt+
  the-pass$ txt+ toward-the(m)$ s" sur" txt+ 50%nullify txt+ ;

: the-water-current$ ( -- ca len )
  s" la" s{ s" caudalosa" s" furiosa" s" fuerte" s" brava" }s txt+
  s" corriente" txt+ s" de agua" 50%nullify txt+ ;

: ^the-water-current$ ( -- ca len )
  the-water-current$ xcapitalized ;

: comes-from$ ( -- ca len )
  s{ s" viene" s" proviene" s" procede" }s ;

: to-keep-going$ ( -- ca len )
  s{ s" avanzar" s" proseguir" s" continuar" }s ;

: lets-you$ ( -- ca len )
  s" te" 50%nullify s" permite" txt+ ;

: narrow-cave-pass$ ( -- ca len )
  s" tramo de cueva" narrow(m)$ rnd2swap txt+ ;
  \ Devuelve una variante de «estrecho tramo de cueva».

: a-narrow-cave-pass$ ( -- ca len )
  s" un" narrow-cave-pass$ txt+ ;
  \ Devuelve una variante de «un estrecho tramo de cueva».

: but|and$ ( -- ca len )
  s{ s" y" but$ }s ;

' but|and$ alias and|but$
: ^but|and$ ( -- ca len )
  but|and$ xcapitalized ;

' ^but|and$ alias ^and|but$
: rocks$ ( -- ca len )
  s{ s" piedras" s" rocas" }s ;

: wanted-peace$ ( -- ca len )
  s{  s" la" s" que haya"
      s" poder" 50%nullify s" vivir en" txt+
      s{ s" tener" s" poder tener" s" poder disfrutar de" }s 50%nullify s" una vida en" txt+
      s" que" s{ s" reine" s" llegue" }s txt+ s" la" txt+
  }s s" paz." txt+ ;
  \ Texto «la paz», parte final de los mensajes «Queremos/Quieren la paz».

: they-want-peace$ ( -- ca len )
  only$ s{ s" buscan" s" quieren" s" desean" s" anhelan" }s txt+
  wanted-peace$ txt+ ;
  \ Mensaje «quieren la paz».

: we-want-peace$ ( -- ca len )
  ^only$ s{ s" buscamos" s" queremos" s" deseamos" s" anhelamos" }s txt+
  wanted-peace$ txt+ ;
  \ Mensaje «Queremos la paz».

: to-understand$ ( -- ca len )
  s{ s" comprender" s" entender" }s ;

: way$ ( -- ca len )
  s{ s" manera" s" forma" }s ;

: to-realize$ ( -- ca len )
  s{ s" ver" s" notar" s" advertir" s" apreciar" }s ;

: more-carefully$ ( -- ca len )
  s{  s" mejor"
      s" con" s{ s" más" s" un" 50%nullify s" mayor" txt+ s" algo más de" }s txt+
        s{ s" detenimiento" s" cuidado" s" detalle" }s txt+
  }s ;

: finally$ ( -- ca len )
  s{  s{ s" al" s" por" }s s" fin" txt+
      s" finalmente"
  }s ;

: ^finally$ ( -- ca len )
  finally$ xcapitalized ;

: rocky(f)$ ( -- ca len )
  s{ s" rocosa" s" de roca" s" s" 50%nullify s+ }s ;

: using$ ( -- ca len )
  s{ s" Con la ayuda de"
     s" Sirviéndote de"
     s" Usando"
     s" Empleando" }s ;


\ vim:filetype=gforth:fileencoding=utf-8
