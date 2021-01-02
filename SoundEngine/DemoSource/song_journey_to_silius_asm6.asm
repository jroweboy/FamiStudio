;this file for FamiTone2 library generated by FamiStudio

journey_to_silius_music_data:
	db 1
	dw @instruments
	dw @samples-3
	dw @song0ch0,@song0ch1,@song0ch2,@song0ch3,@song0ch4
	db <(@tempo_env5), >(@tempo_env5), 0, 0

@instruments:
	dw @env2,@env0,@env9,@env4
	dw @env3,@env0,@env9,@env4
	dw @env12,@env0,@env9,@env8
	dw @env17,@env0,@env9,@env8
	dw @env3,@env0,@env13,@env4
	dw @env7,@env0,@env13,@env8
	dw @env5,@env0,@env10,@env4
	dw @env17,@env0,@env10,@env8
	dw @env6,@env14,@env9,@env8
	dw @env16,@env15,@env9,@env8
	dw @env6,@env15,@env9,@env8
	dw @env1,@env11,@env9,@env8
	dw @env1,@env0,@env9,@env8

@samples:
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;1 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;2 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;3 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;4 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;5 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;6 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;7 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;8 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;9 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;10 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;11 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;12 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;13 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;14 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;15 
	db $10+<(FAMISTUDIO_DPCM_PTR),$3e,$08	;16 (Sample 2)
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;17 
	db $00+<(FAMISTUDIO_DPCM_PTR),$3f,$09	;18 (Sample 1)
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;19 
	db $00+<(FAMISTUDIO_DPCM_PTR),$3f,$0a	;20 (Sample 1)
	db $10+<(FAMISTUDIO_DPCM_PTR),$3e,$0a	;21 (Sample 2)
	db $40+<(FAMISTUDIO_DPCM_PTR),$3f,$0a	;22 (Sample 5)
	db $30+<(FAMISTUDIO_DPCM_PTR),$3e,$0c	;23 (Sample 4)
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;24 
	db $00+<(FAMISTUDIO_DPCM_PTR),$3f,$0c	;25 (Sample 1)
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;26 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;27 
	db $20+<(FAMISTUDIO_DPCM_PTR),$3f,$0d	;28 (Sample 3)
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;29 
	db $10+<(FAMISTUDIO_DPCM_PTR),$3e,$0d	;30 (Sample 2)
	db $20+<(FAMISTUDIO_DPCM_PTR),$3f,$0e	;31 (Sample 3)
	db $00+<(FAMISTUDIO_DPCM_PTR),$3f,$0e	;32 (Sample 1)
	db $10+<(FAMISTUDIO_DPCM_PTR),$3e,$0e	;33 (Sample 2)
	db $40+<(FAMISTUDIO_DPCM_PTR),$3f,$0e	;34 (Sample 5)
	db $30+<(FAMISTUDIO_DPCM_PTR),$3e,$0f	;35 (Sample 4)
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;36 
	db $00+<(FAMISTUDIO_DPCM_PTR),$3f,$0f	;37 (Sample 1)
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;38 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;39 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;40 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;41 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;42 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;43 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;44 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;45 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;46 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;47 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;48 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;49 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;50 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;51 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;52 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;53 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;54 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;55 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;56 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;57 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;58 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;59 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;60 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;61 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;62 
	db $00+<(FAMISTUDIO_DPCM_PTR),$00,$00	;63 

@env0:
	db $c0,$7f,$00,$00
@env1:
	db $04,$cf,$7f,$00,$01
@env2:
	db $08,$ce,$cb,$ca,$c9,$c9,$00,$05,$c1,$c5,$c4,$c3,$c2,$c1,$00,$0d
@env3:
	db $0f,$c4,$c6,$c9,$c8,$0e,$c7,$0e,$c6,$0e,$c5,$0e,$c4,$00,$0c,$c1,$c5,$c4,$c3,$c2,$c1,$00,$14
@env4:
	db $00,$c0,$07,$c1,$c3,$c6,$c3,$c1,$bf,$bd,$ba,$bd,$bf,$00,$03
@env5:
	db $0e,$c5,$c6,$c6,$ca,$cb,$cc,$cb,$ca,$c9,$c8,$c7,$00,$0b,$c1,$c5,$c4,$c3,$c2,$c1,$00,$13
@env6:
	db $00,$cc,$cc,$c9,$c5,$c2,$c0,$00,$06
@env7:
	db $04,$c3,$7f,$00,$01
@env8:
	db $00,$c0,$7f,$00,$01
@env9:
	db $7f,$00,$00
@env10:
	db $c2,$7f,$00,$00
@env11:
	db $c0,$bf,$be,$bd,$bc,$bb,$ba,$b9,$b8,$b7,$00,$09
@env12:
	db $00,$c5,$c9,$c9,$c8,$00,$04
@env13:
	db $c1,$7f,$00,$00
@env14:
	db $c0,$c3,$00,$01
@env15:
	db $c0,$c6,$00,$01
@env16:
	db $00,$cd,$ce,$cc,$c8,$c9,$c7,$c6,$c4,$c3,$c1,$c0,$00,$0b
@env17:
	db $04,$c4,$7f,$00,$01
@env18:
	db $00,$c0,$be,$bc,$bc,$bd,$bf,$c1,$c3,$c4,$c4,$c2,$00,$01
@tempo_env5:
	db $06,$04,$04,$04,$04,$09,$80

@song0ch0:
@ref0:
	db $cf
@song0ch0loop:
@ref1:
	db $fb,<(@tempo_env5),>(@tempo_env5),$00,$a5,$8a,$25,$91,$28,$91,$25,$91,$2a,$2b,$81,$2c,$9f,$2a,$91
@ref2:
	db $82,$20,$af,$f9,$87,$22,$d7,$f9,$87
@ref3:
	db $00,$a5,$8a,$25,$91,$28,$91,$25,$91,$2a,$2b,$81,$2c,$9f,$2a,$91
@ref4:
	db $82,$2c,$af,$f9,$87,$2a,$d7,$f9,$87
	db $ff,$0f
	dw @ref3
	db $ff,$08
	dw @ref2
	db $ff,$0f
	dw @ref3
	db $ff,$08
	dw @ref4
@ref5:
	db $a7,$23,$91,$f9,$91,$22,$87,$f9,$87,$23,$91,$f9,$91,$23,$91
@ref6:
	db $93,$f9,$91,$22,$91,$f9,$91,$22,$87,$f9,$87,$23,$87,$f9,$af
	db $ff,$0f
	dw @ref5
	db $ff,$0f
	dw @ref6
	db $ff,$0f
	dw @ref5
	db $ff,$0f
	dw @ref6
@ref7:
	db $a7,$23,$91,$f9,$91,$22,$87,$f9,$87,$23,$91,$f9,$91,$20,$91
@ref8:
	db $f7,$89,$f9,$87,$8c,$25,$91
@ref9:
	db $89,$f9,$87,$25,$87,$f9,$87,$28,$87,$f9,$87,$25,$87,$f9,$87,$2a,$87,$f9,$87,$25,$91,$f9,$91,$2c,$91
@ref10:
	db $89,$f9,$87,$25,$87,$f9,$87,$2a,$87,$f9,$87,$25,$87,$f9,$87,$28,$87,$f9,$87,$25,$91,$f9,$91,$20,$91
@ref11:
	db $89,$f9,$87,$20,$87,$f9,$87,$23,$87,$f9,$87,$20,$87,$f9,$87,$23,$87,$f9,$87,$26,$62,$27,$81,$62,$28,$8b,$27,$87,$f9,$87,$25,$91
@ref12:
	db $f7,$89,$f9,$87,$25,$91
@ref13:
	db $89,$f9,$87,$25,$87,$f9,$87,$28,$87,$f9,$87,$25,$87,$f9,$87,$2a,$87,$f9,$87,$25,$91,$f9,$91,$2a,$62,$2b,$81,$62,$2c,$8b
@ref14:
	db $93,$25,$87,$f9,$87,$2a,$87,$f9,$87,$25,$87,$f9,$87,$28,$87,$f9,$87,$25,$91,$f9,$91,$20,$91
@ref15:
	db $89,$f9,$87,$20,$87,$f9,$87,$23,$87,$f9,$87,$25,$87,$f9,$87,$26,$62,$27,$81,$62,$28,$8b,$27,$87,$f9,$87,$23,$87,$f9,$87,$25,$91
@ref16:
	db $f7,$9d,$f9,$87
@ref17:
	db $2a,$81,$62,$2b,$81,$62,$2c,$d9,$2a,$87,$f9,$87,$28,$87,$f9,$87,$27,$91
@ref18:
	db $c5,$f9,$87,$28,$91,$f9,$87,$2a,$91,$f9,$87,$2a,$62,$2b,$81,$62,$2c,$8b
@ref19:
	db $f7,$93,$88,$2a,$62,$2b,$81,$62,$2c,$8b
@ref20:
	db $93,$2a,$62,$2b,$81,$62,$2c,$8b,$2e,$91,$f9,$91,$2f,$9b,$f9,$af
@ref21:
	db $8c,$2a,$81,$62,$2b,$81,$62,$2c,$d9,$2a,$87,$f9,$87,$28,$87,$f9,$87,$27,$91
@ref22:
	db $c5,$f9,$87,$28,$91,$f9,$87,$2a,$91,$f9,$87,$25,$91
@ref23:
	db $f7,$a7
@ref24:
	db $93,$80,$25,$87,$f9,$87,$25,$87,$f9,$87,$25,$87,$f9,$87,$25,$87,$f9,$c3
@ref25:
	db $82,$25,$c3,$f9,$87,$25,$c3,$f9,$87
@ref26:
	db $25,$af,$f9,$87,$25,$cd,$f9,$91
@ref27:
	db $25,$c3,$f9,$87,$25,$c3,$f9,$87
	db $ff,$08
	dw @ref26
	db $ff,$08
	dw @ref27
	db $ff,$08
	dw @ref26
@ref28:
	db $27,$c3,$f9,$87,$27,$c3,$f9,$87
@ref29:
	db $27,$af,$f9,$87,$27,$cd,$f9,$91
@ref30:
	db $25,$9b,$f9,$87,$25,$9b,$f9,$87,$25,$91,$f9,$87,$25,$91,$f9,$87,$25,$91
@ref31:
	db $9d,$f9,$87,$25,$9b,$f9,$87,$25,$91,$f9,$87,$25,$91,$f9,$87,$25,$91
	db $ff,$11
	dw @ref31
@ref32:
	db $b1,$f9,$87,$23,$c3,$f9,$87,$25,$91
	db $ff,$11
	dw @ref31
	db $ff,$11
	dw @ref31
@ref33:
	db $9d,$f9,$87,$25,$9b,$f9,$87,$27,$91,$f9,$87,$2c,$91,$f9,$87,$2c,$91
@ref34:
	db $f7,$9d,$f9,$87
@ref35:
	db $00,$a5,$86,$25,$91,$27,$91,$28,$91,$2a,$91,$25,$91,$27,$91
@ref36:
	db $28,$91,$2a,$91,$25,$91,$27,$91,$28,$91,$2a,$91,$25,$91,$27,$91
	db $ff,$10
	dw @ref36
	db $ff,$10
	dw @ref36
	db $ff,$10
	dw @ref36
@ref37:
	db $28,$91,$2a,$91,$27,$91,$28,$91,$2a,$91,$2c,$b9
@ref38:
	db $2f,$91,$31,$a5,$33,$91,$34,$91,$36,$91,$31,$91,$33,$91
@ref39:
	db $34,$91,$36,$a5,$38,$91,$39,$91,$3b,$91,$36,$91,$38,$91
@ref40:
	db $39,$91,$3b,$91,$3d,$f5
@ref41:
	db $f7,$62,$61,$05,$3d,$37,$a5
	db $fd
	dw @song0ch0loop

@song0ch1:
@ref42:
	db $cf
@song0ch1loop:
@ref43:
	db $88,$25,$87,$f9,$87,$28,$87,$f9,$87,$25,$87,$f9,$87,$2a,$62,$2b,$81,$62,$2c,$9f,$2a,$87,$f9,$87,$28,$9b,$f9,$87
@ref44:
	db $23,$af,$f9,$87,$25,$d7,$f9,$87
@ref45:
	db $25,$87,$f9,$87,$28,$87,$f9,$87,$25,$87,$f9,$87,$2a,$62,$2b,$81,$62,$2c,$9f,$2a,$87,$f9,$87,$28,$9b,$f9,$87
@ref46:
	db $2f,$af,$f9,$87,$2e,$d7,$f9,$87
	db $ff,$19
	dw @ref45
	db $ff,$08
	dw @ref44
	db $ff,$19
	dw @ref45
	db $ff,$08
	dw @ref46
@ref47:
	db $a7,$82,$28,$91,$f9,$91,$27,$87,$f9,$87,$28,$91,$f9,$91,$28,$91
@ref48:
	db $93,$f9,$91,$27,$91,$f9,$91,$27,$87,$f9,$87,$28,$87,$f9,$af
@ref49:
	db $a7,$28,$91,$f9,$91,$27,$87,$f9,$87,$28,$91,$f9,$91,$28,$91
	db $ff,$0f
	dw @ref48
	db $ff,$0f
	dw @ref49
	db $ff,$0f
	dw @ref48
@ref50:
	db $a7,$28,$91,$f9,$91,$27,$87,$f9,$87,$28,$91,$f9,$91,$27,$91
@ref51:
	db $f7,$9d,$f9,$87
@ref52:
	db $93,$8e,$25,$b9,$28,$91,$25,$91,$2a,$91,$25,$91
@ref53:
	db $93,$2c,$a5,$25,$91,$2a,$91,$25,$91,$28,$91,$25,$91
@ref54:
	db $93,$20,$b9,$23,$91,$20,$91,$23,$91,$26,$27,$81,$28,$8b
@ref55:
	db $27,$91,$25,$f7,$91
@ref56:
	db $cf,$28,$91,$25,$91,$2a,$91,$25,$91
@ref57:
	db $93,$2a,$2b,$81,$2c,$9f,$25,$91,$2a,$91,$25,$91,$28,$91,$25,$91
@ref58:
	db $93,$20,$b9,$23,$91,$25,$91,$26,$27,$81,$28,$8b,$27,$91
@ref59:
	db $23,$91,$25,$f7,$91
@ref60:
	db $a7,$2a,$81,$2b,$81,$2c,$d9,$2a,$91
@ref61:
	db $28,$91,$27,$e1,$28,$9b,$2a,$87
@ref62:
	db $95,$2b,$81,$2c,$ef,$82,$27,$91
@ref63:
	db $89,$f9,$87,$27,$87,$f9,$87,$2a,$91,$f9,$91,$2c,$9b,$f9,$af
@ref64:
	db $a7,$8e,$2a,$81,$2b,$81,$2c,$d9,$2a,$91
	db $ff,$08
	dw @ref61
@ref65:
	db $93,$25,$f7,$91
@ref66:
	db $93,$80,$20,$87,$f9,$87,$20,$87,$f9,$87,$20,$87,$f9,$87,$20,$87,$f9,$c3
@ref67:
	db $88,$2c,$c3,$f9,$87,$2a,$c3,$f9,$87
@ref68:
	db $28,$af,$f9,$87,$2a,$cd,$f9,$91
@ref69:
	db $2c,$c3,$f9,$87,$2a,$c3,$f9,$87
	db $ff,$08
	dw @ref68
	db $ff,$08
	dw @ref69
	db $ff,$08
	dw @ref68
@ref70:
	db $2d,$c3,$f9,$87,$2c,$c3,$f9,$87
@ref71:
	db $2a,$af,$f9,$87,$2c,$cd,$f9,$91
@ref72:
	db $2c,$9b,$f9,$87,$2a,$9b,$f9,$87,$28,$91,$f9,$87,$2a,$91,$f9,$87,$2c,$91
@ref73:
	db $9d,$f9,$87,$2a,$9b,$f9,$87,$28,$91,$f9,$87,$2a,$91,$f9,$87,$2c,$91
@ref74:
	db $9d,$f9,$87,$2a,$9b,$f9,$87,$28,$91,$f9,$87,$2a,$91,$f9,$87,$28,$91
@ref75:
	db $b1,$f9,$87,$27,$c3,$f9,$87,$2c,$91
	db $ff,$11
	dw @ref73
	db $ff,$11
	dw @ref73
@ref76:
	db $9d,$f9,$87,$2a,$9b,$f9,$87,$2c,$91,$f9,$87,$2f,$91,$f9,$87,$31,$91
@ref77:
	db $f7,$9d,$f9,$87
@ref78:
	db $84,$25,$91,$27,$91,$28,$91,$2a,$91,$25,$91,$27,$91,$28,$91,$2a,$91
@ref79:
	db $25,$91,$27,$91,$28,$91,$2a,$91,$25,$91,$27,$91,$28,$91,$2a,$91
	db $ff,$10
	dw @ref79
	db $ff,$10
	dw @ref79
	db $ff,$10
	dw @ref79
@ref80:
	db $27,$91,$28,$91,$2a,$91,$2c,$b9,$2f,$91,$31,$91
@ref81:
	db $93,$33,$91,$34,$91,$36,$91,$31,$91,$33,$91,$34,$91,$36,$91
@ref82:
	db $93,$38,$91,$39,$91,$3b,$91,$36,$91,$38,$91,$39,$91,$3b,$91
@ref83:
	db $3d,$cd,$63,<(@env18),>(@env18),$cf
@ref84:
	db $cd,$63,<(@env8),>(@env8),$65,$81,$62,$61,$06,$3d,$30,$cd
	db $fd
	dw @song0ch1loop

@song0ch2:
@ref85:
	db $98,$32,$31,$32,$81,$31,$32,$33,$32,$31,$32,$96,$32,$85,$00,$32,$85,$00,$2a,$85,$00,$2a,$85,$00,$26,$8f,$00
@song0ch2loop:
@ref86:
	db $96,$28,$83,$00,$8b,$28,$83,$00,$8b,$2e,$8f,$00,$28,$83,$00,$8b,$28,$83,$00,$8b,$28,$83,$00,$8b,$2e,$8f,$00,$28,$83,$00,$8b
@ref87:
	db $28,$83,$00,$8b,$28,$83,$00,$8b,$2e,$8f,$00,$28,$83,$00,$8b,$28,$83,$00,$8b,$28,$83,$00,$8b,$2e,$8f,$00,$28,$83,$00,$8b
	db $ff,$1e
	dw @ref87
@ref88:
	db $32,$8f,$00,$32,$8f,$00,$2a,$8f,$00,$26,$8f,$00,$32,$85,$00,$32,$85,$00,$89,$32,$85,$00,$2a,$85,$00,$2a,$85,$00,$26,$8f,$00
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
@ref89:
	db $32,$85,$00,$32,$85,$00,$2a,$85,$00,$26,$85,$00,$32,$85,$00,$2a,$85,$00,$2a,$85,$00,$26,$85,$00,$98,$32,$31,$32,$81,$31,$32,$33,$32,$31,$32,$96,$32,$85,$00,$32,$85,$00,$2a,$85,$00,$2a,$85,$00,$26,$85,$00,$26,$85,$00
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1f
	dw @ref88
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$34
	dw @ref89
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1f
	dw @ref88
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$34
	dw @ref89
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1f
	dw @ref88
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
@ref90:
	db $28,$83,$00,$8b,$2e,$8f,$00,$2e,$8f,$00,$2e,$8f,$00,$2e,$8f,$00,$95,$98,$2e,$83,$00,$9d
@ref91:
	db $96,$28,$83,$00,$c7,$28,$83,$00,$c7
@ref92:
	db $28,$83,$00,$b3,$28,$83,$00,$9f,$98,$2e,$83,$00,$8b,$96,$28,$83,$00,$9f
@ref93:
	db $28,$83,$00,$c7,$28,$83,$00,$c7
	db $ff,$10
	dw @ref92
	db $ff,$08
	dw @ref93
	db $ff,$10
	dw @ref92
	db $ff,$08
	dw @ref93
@ref94:
	db $28,$83,$00,$b3,$28,$83,$00,$9f,$2e,$8f,$00,$2e,$8f,$00,$2e,$8f,$00
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1f
	dw @ref88
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$34
	dw @ref89
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1f
	dw @ref88
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$1e
	dw @ref87
	db $ff,$34
	dw @ref89
	db $ff,$1e
	dw @ref87
	db $ff,$34
	dw @ref89
	db $fd
	dw @song0ch2loop

@song0ch3:
@ref95:
	db $92,$26,$83,$26,$85,$26,$83,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87
@song0ch3loop:
@ref96:
	db $90,$1c,$91,$1c,$91,$92,$26,$91,$90,$1c,$91,$1c,$91,$1c,$91,$92,$26,$91,$90,$1c,$91
@ref97:
	db $1c,$91,$1c,$91,$92,$26,$91,$90,$1c,$91,$1c,$91,$1c,$91,$92,$26,$91,$90,$1c,$91
	db $ff,$10
	dw @ref97
@ref98:
	db $94,$26,$91,$26,$91,$26,$91,$26,$91,$26,$87,$26,$91,$26,$87,$26,$87,$26,$87,$26,$91
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
@ref99:
	db $94,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87,$26,$87
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
	db $ff,$14
	dw @ref98
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
	db $ff,$20
	dw @ref99
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
	db $ff,$14
	dw @ref98
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
	db $ff,$20
	dw @ref99
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
	db $ff,$14
	dw @ref98
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
@ref100:
	db $1c,$91,$92,$26,$91,$26,$91,$26,$91,$26,$a7,$26,$a3
@ref101:
	db $90,$1c,$cd,$1c,$cd
@ref102:
	db $1c,$b9,$1c,$a5,$92,$26,$91,$90,$1c,$a5
@ref103:
	db $1c,$cd,$1c,$cd
	db $ff,$08
	dw @ref102
@ref104:
	db $1c,$cd,$1c,$cd
	db $ff,$08
	dw @ref102
@ref105:
	db $1c,$cd,$1c,$cd
@ref106:
	db $1c,$b9,$1c,$a5,$92,$26,$91,$26,$91,$26,$91
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
	db $ff,$14
	dw @ref98
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
	db $ff,$20
	dw @ref99
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
	db $ff,$14
	dw @ref98
	db $ff,$10
	dw @ref96
	db $ff,$10
	dw @ref97
	db $ff,$10
	dw @ref97
	db $ff,$20
	dw @ref99
	db $ff,$10
	dw @ref96
	db $ff,$20
	dw @ref99
	db $fd
	dw @song0ch3loop

@song0ch4:
@ref107:
	db $cf
@song0ch4loop:
@ref108:
	db $19,$91,$19,$91,$19,$91,$19,$91,$19,$91,$19,$91,$19,$91,$19,$91
@ref109:
	db $14,$91,$14,$91,$20,$91,$12,$91,$12,$91,$1e,$91,$12,$91,$1e,$91
	db $ff,$10
	dw @ref108
@ref110:
	db $10,$91,$10,$91,$1c,$91,$12,$91,$12,$91,$1e,$91,$12,$91,$1e,$91
	db $ff,$10
	dw @ref108
	db $ff,$10
	dw @ref109
	db $ff,$10
	dw @ref108
	db $ff,$10
	dw @ref110
	db $ff,$10
	dw @ref108
	db $ff,$10
	dw @ref108
	db $ff,$10
	dw @ref108
	db $ff,$10
	dw @ref108
	db $ff,$10
	dw @ref108
	db $ff,$10
	dw @ref108
@ref111:
	db $19,$91,$19,$91,$19,$91,$19,$91,$19,$91,$19,$91,$19,$91,$14,$91
@ref112:
	db $14,$91,$14,$91,$14,$91,$14,$91,$14,$91,$14,$91,$14,$91,$14,$91
@ref113:
	db $19,$91,$19,$91,$19,$91,$25,$91,$19,$91,$25,$91,$19,$91,$19,$91
@ref114:
	db $17,$91,$23,$91,$17,$91,$17,$91,$17,$91,$23,$91,$17,$91,$17,$91
@ref115:
	db $14,$91,$14,$91,$14,$91,$20,$91,$17,$91,$23,$91,$17,$91,$17,$91
@ref116:
	db $19,$91,$25,$91,$19,$91,$19,$91,$19,$91,$25,$91,$19,$91,$19,$91
	db $ff,$10
	dw @ref113
	db $ff,$10
	dw @ref114
	db $ff,$10
	dw @ref115
	db $ff,$10
	dw @ref116
@ref117:
	db $12,$91,$12,$91,$1e,$91,$12,$91,$12,$91,$12,$91,$1e,$91,$12,$91
@ref118:
	db $14,$91,$14,$91,$20,$91,$14,$91,$14,$91,$14,$91,$20,$91,$14,$91
@ref119:
	db $19,$91,$19,$91,$25,$91,$19,$91,$25,$91,$19,$91,$19,$91,$25,$91
@ref120:
	db $19,$91,$19,$91,$25,$91,$19,$91,$19,$91,$25,$91,$19,$91,$25,$91
	db $ff,$10
	dw @ref117
	db $ff,$10
	dw @ref118
	db $ff,$10
	dw @ref119
@ref121:
	db $19,$91,$19,$91,$19,$91,$19,$91,$19,$cd
@ref122:
	db $19,$cd,$19,$cd
@ref123:
	db $19,$b9,$19,$a5,$19,$91,$25,$91,$19,$91
@ref124:
	db $17,$cd,$17,$cd
@ref125:
	db $17,$b9,$17,$a5,$17,$91,$23,$91,$17,$91
@ref126:
	db $15,$cd,$15,$cd
@ref127:
	db $15,$b9,$15,$a5,$15,$91,$23,$91,$15,$91
@ref128:
	db $17,$cd,$17,$cd
@ref129:
	db $17,$b9,$17,$a5,$23,$91,$17,$91,$23,$91
	db $ff,$10
	dw @ref120
@ref130:
	db $17,$91,$17,$91,$23,$91,$17,$91,$17,$91,$23,$91,$17,$91,$23,$91
@ref131:
	db $15,$91,$15,$91,$21,$91,$15,$91,$15,$91,$21,$91,$15,$91,$21,$91
	db $ff,$10
	dw @ref130
	db $ff,$10
	dw @ref120
	db $ff,$10
	dw @ref130
	db $ff,$10
	dw @ref131
	db $ff,$10
	dw @ref120
@ref132:
	db $19,$91,$19,$91,$19,$91,$25,$91,$19,$91,$19,$91,$23,$91,$25,$91
@ref133:
	db $17,$91,$17,$91,$17,$91,$23,$91,$17,$91,$17,$91,$21,$91,$23,$91
@ref134:
	db $16,$91,$16,$91,$16,$91,$22,$91,$16,$91,$16,$91,$14,$91,$22,$91
@ref135:
	db $15,$91,$15,$91,$15,$91,$21,$91,$15,$91,$15,$91,$1f,$91,$21,$91
	db $ff,$10
	dw @ref132
	db $ff,$10
	dw @ref133
	db $ff,$10
	dw @ref134
	db $ff,$10
	dw @ref135
	db $ff,$10
	dw @ref132
	db $ff,$10
	dw @ref120
	db $fd
	dw @song0ch4loop
