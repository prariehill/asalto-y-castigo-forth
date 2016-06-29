\ entity_identifiers.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606291146

\ Note: The comments of the code are in Spanish.

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

entity: ulfius~
' ulfius~ is protagonist~  \ Actualizar el vector que apunta al ente protagonista

\ Entes que son (seudo)personajes:
entity: ambrosio~
entity: (leader~) ' (leader~) is leader~
entity: soldiers~
entity: refugees~
entity: officers~

\ Entes que son objetos:
entity: altar~
entity: arch~
entity: bed~
entity: bridge~
entity: candles~
entity: cave-entrance~
entity: cloak~
entity: cuirasse~
entity: door~
entity: emerald~
entity: fallen-away~
entity: flags~
entity: flint~
entity: grass~
entity: idol~
entity: key~
entity: lake~
entity: lock~
entity: (log~) ' (log~) is log~
entity: piece~
entity: rags~
entity: ravine-wall~
entity: rocks~
entity: snake~
entity: (stone~) ' (stone~) is stone~
entity: (sword~) ' (sword~) is sword~
entity: table~
entity: thread~
entity: (torch~) ' (torch~) is torch~
entity: wall~  \ XXX TODO -- inconcluso
entity: waterfall~

\ Entes escenario (en orden de número):
entity: (location-01~) ' (location-01~) is location-01~
entity: location-02~
entity: location-03~
entity: location-04~
entity: location-05~
entity: location-06~
entity: location-07~
entity: location-08~
entity: location-09~
entity: location-10~
entity: location-11~
entity: location-12~
entity: location-13~
entity: location-14~
entity: location-15~
entity: location-16~
entity: location-17~
entity: location-18~
entity: location-19~
entity: location-20~
entity: location-21~
entity: location-22~
entity: location-23~
entity: location-24~
entity: location-25~
entity: location-26~
entity: location-27~
entity: location-28~
entity: location-29~
entity: location-30~
entity: location-31~
entity: location-32~
entity: location-33~
entity: location-34~
entity: location-35~
entity: location-36~
entity: location-37~
entity: location-38~
entity: location-39~
entity: location-40~
entity: location-41~
entity: location-42~
entity: location-43~
entity: location-44~
entity: location-45~
entity: location-46~
entity: location-47~
entity: location-48~
entity: location-49~
entity: location-50~
entity: location-51~

\ Entes globales:
entity: sky~
entity: floor~
entity: ceiling~
entity: clouds~
entity: cave~  \ XXX TODO -- inconcluso

\ Entes virtuales
\ (necesarios para la ejecución de algunos comandos):
entity: inventory~
entity: (exits~)  ' (exits~) is exits~
entity: north~
entity: south~
entity: east~
entity: west~
entity: up~
entity: down~
entity: out~
entity: in~
entity: enemy~

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

\ vim:filetype=gforth:fileencoding=utf-8