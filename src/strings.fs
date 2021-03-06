\ strings.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201711072221

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Gforth

require random.fs

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/bracket-false.fs           \ `[false]`
require galope/fifty-percent-nullify.fs   \ `50%nullify`
require galope/null-dollar.fs             \ `null$`
require galope/question-question.fs       \ `??`
require galope/s-curly-bracket.fs         \ `s{`
require galope/replaced.fs                \ `replaced`
require galope/stringer.fs                \ Circular string buffer
require galope/txt-plus.fs                \ `txt+`
require galope/x-capitalized.fs           \ `xcapitalized`
require galope/x-conversions.es.fs        \ `xconversions` Spanish table

\ Forth Foundation Library
\ http://irdvo.github.io/ffl/

require ffl/str.fs

set-current

\ ==============================================================

str-create tmp-str
  \ Cadena dinámica de texto temporal para usos variados.

: str-get-last-char ( a -- c )
  dup str-length@ 1- swap str-get-char ;
  \ Devuelve el último carácter de una cadena dinámica.
  \ XXX TODO -- soporte para UTF-8

: str-get-last-but-one-char ( a -- c )
  dup str-length@ 2 - swap str-get-char ;
  \ Devuelve el penúltimo carácter de una cadena dinámica.
  \ XXX TODO -- soporte para UTF-8

: -punctuation ( ca len -- ca len )
  exit \ XXX TMP
  2dup bounds  ?do
    i c@ chr-punct? if  bl i c!  then
  loop ;
  \ Sustituye por espacios todos los signos de puntuación ASCII de una cadena.
  \ XXX TODO -- recorrer la cadena por caracteres UTF-8
  \ XXX TODO -- sustituir también signos de puntuación UTF-8
  \ XXX FIXME -- no eliminar las marcas "#" de los comandos del sistema

: tmp-str! ( ca len -- ) tmp-str str-set ;
  \ Guarda una cadena en la cadena dinámica `tmp-str`.

: tmp-str@ ( -- ca len ) tmp-str str-get ;
  \ Devuelve el contenido de cadena dinámica `tmp-str`.

: *>verb-ending ( ca len f -- )
  [false] [if]  \ Versión al estilo de BASIC:
    if  s" n"  else  s" "  then  s" *" replaced
  [else]  \ Versión sin estructuras condicionales, al estilo de Forth:
    s" n" rot and  s" *" replaced
  [then] ;
  \ Cambia por «n» (terminación verbal en plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular los verbos de una frase.
  \ ca len = Texto
  \ f = ¿Hay que poner los verbos en plural?

: *>plural-ending ( ca len f -- )
  [false] [if]  \ Versión al estilo de BASIC:
    if  s" s"  else  s" "  then  s" *" replaced
  [else]  \ Versión sin estructuras condicionales, al estilo de Forth:
    s" s" rot and  s" *" replaced
  [then] ;
  \ Cambia por «s» (terminación plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular los verbos de una frase.
  \ ca len = Expresión
  \ f = ¿Hay que poner los verbos en plural?
  \ XXX TODO -- no usado

: space+ ( ca1 len1 -- ca2 len2 ) s"  " s+ ;
  \ Añade un espacio al final de una cadena.

: period+ ( ca1 len1 -- ca2 len2 ) s" ." s+ ;
  \ Añade un punto al final de una cadena.

: comma+ ( ca1 len1 -- ca2 len2 ) s" ," s+ ;
  \ Añade una coma al final de una cadena.

: colon+ ( ca1 len1 -- ca2 len2 ) s" :" s+ ;
  \ Añade dos puntos al final de una cadena.

: hyphen+ ( ca1 len1 -- ca2 len2 ) s" -" s+ ;
  \ Añade un guion a una cadena.

: and& ( ca1 len1 -- ca2 len2 ) s" y" txt+ ;
  \ Añade una conjunción «y» al final de una cadena.
  \ XXX TODO -- no usado

: or& ( ca1 len1 -- ca2 len2 ) s" o" txt+ ;
  \ Añade una conjunción «o» al final de una cadena.
  \ XXX TODO -- no usado

: rnd2swap ( ca1 len1 ca2 len2 -- ca1 len1 ca2 len2 | ca2 len2 ca1 len1 )
  2 random ?? 2swap ;
  \ Intercambia (con 50% de probabilidad) la posición de dos textos.

: (both) ( ca1 len1 ca2 len2 -- ca1 len1 ca3 len3 ca2 len2 | ca2 len2 ca3 len3 ca1 len1 )
  rnd2swap s" y" 2swap ;
  \ Devuelve las dos cadenas recibidas, en cualquier orden,
  \ y separadas en la pila por la cadena «y».

: both ( ca1 len1 ca2 len2 -- ca3 len3 )
  (both) txt+ txt+ ;
  \ Devuelve dos cadenas unidas en cualquier orden por «y».
  \ Ejemplo: si los parámetros fueran «espesa» y «fría»,
  \ los dos resultados posibles serían: «fría y espesa» y «espesa y fría».

: both& ( ca0 len0 ca1 len1 ca2 len2 -- ca3 len3 )
  both txt+ ;
  \ Devuelve dos cadenas unidas en cualquier orden por «y»; y concatenada (con separación) a una tercera.

: both? ( ca1 len1 ca2 len2 -- ca3 len3 )
  (both) txt+ 50%nullify txt+ ;
  \ Devuelve al azar una de dos cadenas,
  \ o bien ambas unidas en cualquier orden por «y».
  \ Ejemplo: si los parámetros fueran «espesa» y «fría»,
  \ los cuatro resultados posibles serían:
  \ «espesa», «fría», «fría y espesa» y «espesa y fría».

: both?& ( ca0 len0 ca1 len1 ca2 len2 -- ca3 len3 )
  both? txt+ ;
  \ Concatena (con separación) al azar una de dos cadenas
  \ (o bien ambas unidas en cualquier orden por «y») a una tercera cadena.

: both?+ ( ca0 len0 ca1 len1 ca2 len2 -- ca3 len3 )
  both? s+ ;
  \ Concatena (sin separación) al azar una de dos cadenas
  \ (o bien ambas unidas en cualquier orden por «y») a una tercera cadena.

\ vim:filetype=gforth:fileencoding=utf-8

