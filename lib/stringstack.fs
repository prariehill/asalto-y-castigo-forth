\ stringstack v0.10

\ download as http://forthfreak.net/stringstack

\ warnings dup @ swap off
\ warnings !


\ 2017-07-31: Modified by Marcos Cruz (programandala.net) to make it
\ compatible with Gforth 0.7.9: Conditional compilation of `cell-` and
\ `cell/`; rename `stack` `$stack`; rename `stack:` `$stack:`.

\ strings.f   string words  (should be) ANS conform. compiles with vanilla gforth
\ v0.10  20050107 Speuler  added -scan$, -skip$, searchn$ and dropn$
\ v0.09a 20041008 Speuler  added scan$ skip$ description
\ v0.09  20020305 Speuler  added scan$ skip$
\ v0.08, 20020211 Speuler  added mid$  reverse$  translate$
\ v0.07, 20020211 Speuler  improved left$, right$, split$, pick$, roll$, .s$, constants for throw values
\ v0.06, 20020211 Speuler  fixed bug in example, speeded up dup$ drop$ swap$ over$, added left$ right$
\ v0.05, 20020210 Speuler  added split$  merge$
\ v0.04, 20020210 Speuler  added compare$  roll$  search$  subsearch$
\ v0.03, 20020210 Speuler  added depth$  .s$  pick$
\ v0.02, 20020210 Speuler  factored out refcount decrementing, pushing to flushstrings
\ v0.01, 20020210 Speuler  initial implementation




\ stringstack words:
\  tos$     ( -- a n )     gives topmost string, same as 0 pick$ (but no test whether topmost elements actually exists)
\  push$    ( a n -- )     pushs a string to stringstack
\  pop$     ( -- a n )     pops a string from stringstack, marks it as freeable if last ref
\  dup$     ( -- )         duplicates string on stringstack
\  drop$    ( -- )         drops a string on stringstack, marks as freeable if last ref
\  dropn$   ( n -- )       drop top n strings
\  swap$    ( -- )         swaps top two strings on stringstack
\  over$    ( -- )         pushs a copy of nos string
\  free$    ( -- )         frees memory used by freeable strings
\  depth$   ( -- n )       number of items on string stack
\  compare$ ( n1 n2 -- n3 ) compare strings at stack pos n1 and n2
\  pick$    ( n1 -- a n2 ) return nth string, counting from top of string stack
\  roll$    ( n -- )       roll string at string stack pos n to top of string stack
\  searchn$ ( a n1 n2 -- n3 -1 | 0 ) search for a n1 through n2 elements
\  search$  ( a n -- n -1 | 0 )  search through stringstack, return stack position of match, or 0
\  subsearch ( a n -- n -1 | 0 ) substring search through stringstack.
\  left$    ( n -- )       leaves n left chars, or cuts off -n right chars
\  right$   ( n -- )       leaves n right chars, or cuts off -n left chars
\  mid$     ( index len -- ) extracts string subsection. negative index counts from the right.
\  reverse$ ( -- )         mirror image of string
\  split$   ( n -- )       splits top string into two at position n. n<0 counts fromon string end
\  merge$ ( -- )           appends top string to nos string
\  translate$ ( a n -- )   replace chars in string against chars from table at a
\  skip$    ( c -- n )     returns length of string after skipping leading cs
\  scan$    ( c -- n )     returns length of string from first c to string end
\ -scan$    ( c -- n )     reverse scan, from right end of string
\ -skip$    ( c -- n )     reverse skip, from right end of string
\  .s$      ( -- )         display stack dump of string stack. number shown is string reference count

\ string count is cell size, i.e. strings > 255 bytes are ok.
\ split$ and merge$ have been implemented to avoid having to use length-limited strings words


base @ decimal
1024 constant maxstrings


\ ---------- general stuff ----------

\ throw values
 -4 constant stack_underflow    \ string stack underflow
-24 constant invalid_argument   \ pick$, roll$ index too high
 32 constant maxtype            \ max chars per string typed by .s$

[undefined] cell/ [if]

cell 2 = [if]  ' 2/ alias cell/  ( n1 -- n2 )   [then]
cell 4 = [if] : cell/ ( n1 -- n2 )        2 rshift ; [then]
cell 8 = [if] : cell/ ( n1 -- n2 )        3 rshift ; [then]

[then]

[undefined] cell- [if]
  : cell- ( x1 -- x2 )   cell -  ;
[then]

  : inc   ( a -- )   1 swap +!  ;
  : dec   ( a -- )   -1 swap +!  ;
  : skim  ( a1 -- a2 x )    cell+ dup cell- @  ;
  : pluck ( x1 x2 x3 -- x1 x2 x3 x1 )   2 pick  ;
  : 3dup ( x1 x2 x3 -- x1 x2 x3 x1 x2 x3 )  pluck pluck pluck ;
  : exchange ( x1 a -- x2 )     dup @ -rot ! ;
  : swapchars ( a1 a2 -- )   dup >r c@  swap dup c@  r> c! c! ;




\ builds stack with structure   maxdepth, depth, stackdata.
\ expects that stack space has been allocated already at a
\ depth and maxdepth are given in bytes.
: $stack  ( n a -- )                       0 over cell+ ! !  ;


: $stack:  ( n -- )                        create here over cell+ cell+ allot $stack  ;
: sp   ( a1 -- a2 )                       cell+ dup @ + ;          \ return address of top stack element
: push ( x a -- )                         cell+ cell over +! dup @ + !  ;

: pop  ( a -- x )
   cell+  dup >r
   dup @
   dup 0<=
   if
      stack_underflow throw
   then
   + @                        \ read stacked data.
   [ cell negate ] literal
   r> +!                      \ unbump stack pointer
;

: stackused ( a -- n )        cell+ @ cell/ ;    \ given a stack, returns depth
: stackfree ( a -- n )        skim swap @ - cell/ ;  \ given a stack, returns free



\ --------------- string stack stuff -------------------


maxstrings cells $stack: stringstack
maxstrings cells $stack: flushstack


: depth$ ( -- n )    stringstack stackused ;
: 'tos$  ( -- a )     stringstack sp ;               \ returns address of top element in string stack
: tos$ ( -- a n )    'tos$ @ cell+ skim  ;           \ same as 0 pick$


\ allocates space for refcount, stringlen, string
\ refcount and stringlen are cell size
: alloc$  ( len -- addr 0 | 0 err )   cell+ cell+ allocate  ;

\ push string to flushstrings if refcount is 0. decrement refcount
: ?free$  ( a -- )
   dup @ 0= if                 \ refcount = 0 ?
      dup flushstack push      \ string freeable
   then
   dec                         \ decrement refcount
;

: assure_valid_index ( n -- )  depth$ u>= if invalid_argument throw then ;

\ releases unused string space. right now there is the risk of
\ flushstack overflow. you need to call free$ before that happens.
: free$ ( a -- 0 | err )   flushstack stackused  0 ?do flushstack pop free throw loop ;

: push$  ( a n -- )
   dup alloc$ throw        \ a1 n a2
   dup off                 \ set refcount
   dup stringstack push
   cell+ 2dup !            \ set stringlen
   cell+ swap move         \ copy string
;


: pop$  ( -- a n )   stringstack pop  dup ?free$  cell+ skim  ;



\ ------------------- string stack primitives -------------------
\ (calling them primitives because there exist data stack, non-string equivalents for these)



: drop$  ( -- )     stringstack pop ?free$  ;
: dropn$ ( n -- )   0 ?do  drop$  loop ;
: dup$  ( -- )      'tos$ @  dup inc   stringstack push  ;

: swap$  ( -- )   'tos$ cell- dup skim   swap exchange swap ! ;
: over$  ( -- )   'tos$ cell- @ dup inc  stringstack push ;


\ return the nth string from top of string stack as address/count.
\ beware that pick$ does NOT put the nth string on top of string stack.
: pick$  ( n -- a n )   dup assure_valid_index   cells negate   'tos$ + @  cell+ skim  ;


: roll$   ( n -- )
   dup assure_valid_index
   cells 'tos$ dup >r             \ address tos, keep
   over - dup @ >r                \ read target string handle
   cell+ dup cell- rot move       \ move all down
   r> r> !                        \ write rolled string to tos
;



\ compares string1 at stack pos n1 with string2 at n2, returns -1 if
\ string1, string2 are in descending order, 0 if strings are identical,
\ 1 if string1, string2 are in ascending order.
: compare$ ( n1 n2 -- -1 | 0 | 1 )   >r pick$  r> pick$  compare ;


\ -------------- more operations on stacked strings ----------------



\ show string stack dump. first number is string reference count
: .s$  ( -- )
   depth$ 0 ?do
      cr i pick$
      over cell- cell- @ .        \ ref count
      tuck maxtype min
      tuck type
      - ?dup if                   \ string was truncated
         ." ... +" .              \ indicate "there's more"
      then
   loop
;


\ n gives len of remainder of string incl char scanned for
: skip$ ( c -- n )   tos$ rot skip nip ;

\ n gives len of remainder of string incl char scanned for
: scan$ ( c -- n )   tos$ rot scan nip ;

\ search for last occurance of c
: -scan$  ( c -- n ) tos$ over >r tuck + swap 0 ?do 2dup 1- c@ = ?leave 1- loop nip r> - ;

\ returns len of remaining string, after having skipped any c at the end of the string
: -skip$  ( c -- n ) tos$ over >r tuck + swap 0 ?do 2dup 1- c@ <> ?leave 1- loop nip r> - ;



\ seperate string stack top at bl into words
\ : scanskipdemo ( a n -- )
\   begin
\      bl scan$                \ search next space
\   ?dup while                 \ space found:
\      negate split$           \   split string at space
\      bl skip$ right$         \   cut off leading space
\   repeat ;



\ search for string a n1 in top n2 string stack elements
: searchn$  ( a n1 n2 -- n -1 | 0 )
   begin dup
   while
      1- 3dup pick$ compare
      0= if
         nip nip true
   exit
      then
   repeat
   nip nip
;

: search$  ( a n1 -- n2 -1 | 0 )  depth$ searchn$ ;


: subsearch$  ( a n1 -- n2 -1 | 0 )
   depth$
   begin dup
   while
      1- 3dup pick$
      pluck over u>
      if
         2drop 2drop true
      else
         drop over compare
      then
      0= if
         nip nip true
   exit
      then
   repeat
   nip nip
;

\ appends tos string to nos string
: merge$  ( -- )   pop$ >r pop$ tuck r@ + push$ 'tos$ @ cell+ cell+ +  r> move ;


\ splits string on stringstack into two strings at position n.
\ also accepts negative index, which counts from end of string.
\ index out of bounds will be truncated to string boundary.
: split$  ( n -- )
   >r pop$
   r@ 0< if
      dup r> + 0 max >r
   then
   dup r> min
   pluck over push$
   /string push$
;




\ if top string is referenced more than once, detach it, and create a single-ref copy
\ returns address and len of top string
\ used before in-sito modification of top string, like reverse$
: detach$  ( -- a n )
   'tos$ @ @         ( refcount )
   if                ( multiple references )
      pop$ push$     ( create physical duplicate of string )
   then  tos$ ;


\ helper word for left$ and right$
: clipped   ( n1 n2 n3-- n4 )   0< if  + 0 max  else  min  then ;

\ n>=0 : leaves left n chars of string
\ n<0 :  cuts -n chars off the end of string
\ index out of bounds will be truncated to string boundary.
: left$  ( n -- )   >r pop$ r> dup clipped push$ ;

\ n>=0 : leaves right n chars of string
\ n<0 :  cuts -n chars off the left of string
\ index out of bounds will be truncated to string boundary.
: right$ ( n -- )   >r pop$ dup r> 2dup clipped - /string push$ ;

\ extracts string subsection.
\ index>=0: start counting from left. index<0: start counting from right.
\ index or len out of bounds will be truncated to string boundary.
: mid$ ( index len -- )  swap ?dup if negate right$ then  0 max left$ ;

: reverse$ ( -- )    detach$ dup 2/ 0 ?do 1- 2dup over + swap i + swapchars loop  2drop ;

\ pass a translation table, starting with ascii 0, of length n.
\ each character in top string is replaced against the corresponding character from table.
: translate$   ( a n -- )
   detach$
   bounds ?do
      dup i c@ u>                                  \ string character in table ?
      if
         over i c@ + c@                            \ read table character
   i c!                                      \ store in string
      then
   loop
   2drop  ;


\ example tables:
\ create 1to1  128 0 [do] [i] c, [loop]                       \ tables contains chars 0...127
\ '_  1to1 bl + c!                                            \ replace space against underscore in translation table
\    1to1 128 translate$                                      \ replace spaces in top string against underscores
\ bl 1to1 bl + c!                                             \ fix table 1 to 1 again, as we'll reuse it for example 3

\ create noctrlchars  here 32 dup allot bl fill               \ creates table with 32 spaces
\  noctrlchars 32 translate$                                  \ translates control chars against spaces

\ 1to1 'a +   1to1 'A +   26 move                             \ lowercast capitals in table
\ 1to1 'Z 1+ translate$                                       \ lowercast string



\ ------------------------------------------------------------

base !
