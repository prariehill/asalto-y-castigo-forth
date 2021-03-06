\ answers.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201711171345

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/fifty-percent-nullify.fs   \ `50%nullify`
require galope/one-minus-store.fs         \ `1-!`
require galope/one-plus-store.fs          \ `1+!`
require galope/question-execute.fs        \ `?execute`
require galope/question-question.fs       \ `??`
require galope/s-curly-bracket.fs         \ `s{`
require galope/txt-plus.fs                \ `txt+`

set-current

\ ==============================================================

\ Para los casos en que el programa hace una pregunta que debe
\ ser respondida con «sí» o «no», usamos un truco análogo al
\ del vocabulario del juego: creamos una lista de palabras
\ con palabras cuyos nombres sean las posibles respuestas:
\ «sí», «no», «s» y «n».  Estas palabras actualizarán una
\ variable,  con cuyo valor el programa sabrá si se ha
\ producido una respuesta válida o no y cuál es.
\
\ En principio, si el jugador introdujera varias respuestas
\ válidas la última sería la que tendría efecto. Por ejemplo,
\ la respuesta «sí sí sí sí sí no» sería considerada negativa.
\ Para dotar al método de una chispa de inteligencia, las
\ respuestas no cambian el valor de la variable sino que lo
\ incrementan o decrementan. Así el mayor número de respuestas
\ afirmativas o negativas decide el resultado; y si la
\ cantidad es la misma, como por ejemplo en «sí sí no no», el
\ resultado será el mismo que si no se hubiera escrito nada.

variable #answer
  \ Su valor será 0 si no ha habido respuesta válida; negativo para
  \ «no»; y positivo para «sí»

: answer-undefined ( -- ) #answer off ;
  \ Inicializa la variable antes de hacer la pregunta.

: think-it-again$ ( -- ca len )
  s{ s" Piénsalo mejor"
  s" Decídete" s" Cálmate" s" Concéntrate"
  s" Presta atención"
  s{ s" Prueba" s" Inténtalo" }s again$ txt+
  s" No es tan difícil" }s colon+ ;
  \ Devuelve un mensaje complementario para los errores.

: yes-but-no$ ( -- ca len )
  s" ¿Primero «sí»" but|and$ txt+
  s" después «no»?" txt+ think-it-again$ txt+ ;
  \ Devuelve mensaje de error: se dijo «no» tras «sí».

: no-but-yes$ ( -- ca len )
  s" ¿Primero «no»" but|and$ txt+
  s" después «sí»?" txt+ think-it-again$ txt+ ;
  \ Devuelve mensaje de error: se dijo «sí» tras «no».

: yes-but-no.error ( -- ) yes-but-no$ language-error ;
  \ Muestra error: se dijo «no» tras «sí».

: no-but-yes.error ( -- ) no-but-yes$ language-error ;
  \ Muestra error: se dijo «sí» tras «no».

: two-options-only$ ( -- ca len )
  ^only$ s{ s" hay" s" tienes" }s txt+
  s" dos" txt+ s" respuestas" s" posibles" rnd2swap txt+ txt+ colon+
  s" «sí»" s" «no»" both& s" (o sus iniciales)" txt+ period+ ;
  \ Devuelve un mensaje que informa de las opciones disponibles.

: two-options-only.error ( -- ) two-options-only$ language-error ;
  \ Muestra error: sólo hay dos opciones.

: wrong-yes$ ( -- ca len )
  s{ s" ¿Si qué...?" s" ¿Si...?" s" ¿Cómo «si»?" s" ¿Cómo que «si»?" }s
  s" No" txt+ s{
  s{ s" hay" s" puedes poner" }s s{ s" condiciones" s" condición alguna" }s txt+
  s{ s" hay" s" tienes" }s s" nada que negociar" txt+ }s txt+
  s{ s" aquí" s" en esto" s" en esta cuestión" }s txt+ period+
  \ two-options-only$ 50%nullify txt+  \ XXX TODO
 ;
  \ Devuelve el mensaje usado para advertir de que se ha escrito mal «sí».

: wrong-yes.error ( -- ) wrong-yes$ language-error ;
  \ Muestra error: se ha usado la forma errónea «si».

: error-if-previous-yes ( -- )
  #answer @ 0> ?? yes-but-no.error ;
  \ Provoca error si antes había habido síes.

: answer-no ( -- ) error-if-previous-yes  #answer 1-! ;
  \ Anota una respuesta negativa.

: error-if-previous-not ( -- )
  #answer @ 0< ?? no-but-yes.error ;
  \ Provoca error si antes había habido noes.

: answer-yes ( -- ) error-if-previous-not  #answer 1+! ;
  \ Anota una respuesta afirmativa.

wordlist  dup constant answer-wordlist  set-current

: sí ( -- ) answer-yes ;
: s  ( -- ) answer-yes ;
: no ( -- ) answer-no ;
: n  ( -- ) answer-no ;
: si ( -- ) wrong-yes.error ;

restore-wordlists

: yes|no ( ca len -- n )
  answer-undefined
  answer-wordlist 1 set-order
  ['] evaluate-command catch
  dup if  nip nip  then  \ Reajustar la pila si ha habido error
  dup ?execute 0=  \ Ejecutar el posible error y preparar su indicador para usarlo en el resultado
  #answer @ 0= ?? two-options-only.error
  #answer @ dup 0<> and and  \ Calcular el resultado final
  restore-wordlists ;
  \ Evalúa una respuesta _ca len_ a una pregunta del tipo «sí o no»,
  \ devolviendo el resultado _n_ (un número negativo para «no» y
  \ positivo para «sí»; cero si no se ha respondido ni «sí» ni «no», o
  \ si se produjo un error).

: .question ( xt -- )
  question-color execute /ltype ;
  \ Imprime la pregunta cuyo texto devuelve la ejecución de _xt_.

: accept-answer ( -- ca len ) answer-wordlist accept-input ;
  \ Espera una respuesta sí/no del jugador y la devuelve sin espacios
  \ laterales y en minúsculas en la cadena _ca len_.

: answer ( xt -- n )
  begin  dup .question accept-answer  yes|no ?dup
  until  nip ;
  \ Devuelve la respuesta a una pregunta del tipo «sí o no» (cuyo
  \ texto devuelve la ejecución de _xt_), devolviendo la respuesta
  \ _n_: un número negativo para «no» y positivo para «sí»

: yes? ( xt -- f ) answer 0> ;
  \ ¿Es afirmativa la respuesta a una pregunta binaria cuyo texto
  \ es imprimido por _xt_?

: no? ( xt -- f ) answer 0< ;
  \ ¿Es negativa la respuesta a una pregunta binaria cuyo texto
  \ es imprimido por _xt_?

\ ==============================================================
\ Change log

\ 2017-11-16: Update to Galope 0.138.1: replace `++` with `1+!`, and
\ `--` with `1-!`.
\
\ 2017-11-17: Update from Galope's deprecated module <print.fs> to
\ <l-type.fs>.

\ vim:filetype=gforth:fileencoding=utf-8
