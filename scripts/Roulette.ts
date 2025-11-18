interface Wager {
    wagerAmount: number;
    isPremiumToken: boolean;
}

interface DrawResult {
    value: number;
}

interface PlayResponse {
    wager: Wager;
    draw: DrawResult;
}

async function PlayFetch(
    wagerAmount: number,
    betType: number,
    selected: number[]
): Promise<{ ok: boolean; body: PlayResponse | string }> {
    const payload = {
        wager: { wagerAmount, isPremiumToken: false },
        rouletteChoice: { betType, selected },
    };

    return fetch('/api/games/roulette/play', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
    }).then(async response => {
        const data: PlayResponse | string = response.ok
            ? await response.json()
            : await response.text();

        return { ok: response.ok, body: data };
    });
}

// Alpine component typing
interface RouletteComponent {
    betType: number;
    numbersInput: string;
    wagerAmount: number;
    loading: boolean;
    result: PlayResponse | null;
    error: string;
    selectedNumbers: number[];
    play: () => Promise<void>;
    rouletteGrid: () => NumberColor[];
    selectNumber: (num: number) => void;
    isSelected: (num: number) => boolean;
    animateRoulette: (finalNumber: number) => Promise<void>;
    getButtonClass: (num: number, color: string) => string;
    highlightedNumber: number
}

// Factory used by Alpine: x-data="rouletteComponent()"
function rouletteComponent(initialBetType: number = 0): RouletteComponent {
    return {
        betType: initialBetType,
        numbersInput: '',
        wagerAmount: 10,
        loading: false,
        result: null,
        error: '',
        selectedNumbers: [] as number[],
        highlightedNumber: null,
        
        getButtonClass(num: number, color: string): string {
            if (this.highlightedNumber === num) {
                return 'bg-warning';
            }
            if (this.result && this.result.draw.value === num && this.highlightedNumber === -1) {
                return 'bg-primary';
            }
            return color === 'red' ? 'bg-danger' : (num === 0 ? 'bg-success' : 'bg-dark');
        },
        
        async animateRoulette(finalNumber: number): Promise<void> {
            const numbers = [0, ...Array.from({ length: 36 }, (_, i) => i + 1)];
            const delayPerNumber = 3500 / numbers.length;
            const iterations = 2;

            for (let iter = 0; iter < iterations; iter++) {
                for (const num of numbers) {
                    this.highlightedNumber = num;
                    await new Promise(resolve => setTimeout(resolve, delayPerNumber));
                    
                    if (iter === iterations - 1 && num === finalNumber) {
                        this.highlightedNumber = -1; 
                        return;
                    }
                }
            }
        },
        
        rouletteGrid: () => {
            const redNumbers = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];

            const numbers: NumberColor[] = [];
            for (let num = 1; num <= 36; num++) {
                numbers.push({
                    value: num,
                    color: redNumbers.includes(num) ? 'red' : 'black'
                });
            }

            return numbers;
        },

        selectNumber(num: number) {
            const BetType = {
                STRAIGHT_UP: 0,
                SPLIT: 1,
                STREET: 2,
                RED_OR_BLACK: 3,
                ODD_OR_EVEN: 4
            };

            switch(this.betType) {
                case BetType.STRAIGHT_UP:
                case BetType.RED_OR_BLACK:
                case BetType.ODD_OR_EVEN:
                    this.selectedNumbers = [num];
                    break;

                case BetType.STREET:
                    const columnStart = Math.floor((num - 1) / 3) * 3 + 1;
                    this.selectedNumbers = [columnStart, columnStart + 1, columnStart + 2];
                    break;

                case BetType.SPLIT:
                    if (this.selectedNumbers.includes(num)) {
                        this.selectedNumbers = this.selectedNumbers.filter(n => n !== num);
                    }
                    else if (this.selectedNumbers.length >= 2) {
                        this.selectedNumbers = [this.selectedNumbers[0], num];
                    }
                    else {
                        this.selectedNumbers = [...this.selectedNumbers, num];
                    }
                    break;
            }

            this.numbersInput = this.selectedNumbers.join(',');
        },

        isSelected(num: number): boolean {
            return this.selectedNumbers.includes(num);
        },

        async play() {
            this.loading = true;
            this.error = '';
            this.result = null;

            try {
                const selected = this.numbersInput
                    .split(',')
                    .map(n => n.trim())
                    .filter(n => n !== '')
                    .map(n => parseInt(n, 10))
                    .filter(n => !Number.isNaN(n));

                const result = await PlayFetch(this.wagerAmount, this.betType, selected);

                if (!result.ok) {
                    this.error = `Error: ${result.body}`;
                    return;
                }

                this.result = result.body as PlayResponse;

                await this.animateRoulette(this.result.draw.value);

                setTimeout(() => {
                    window.location.reload();
                }, 1000);
            } catch (e: unknown) {
                const msg = e instanceof Error ? e.message : String(e);
                this.error = 'Unexpected error: ' + msg;
            } finally {
                this.loading = false;
            }
        },
    };
}

interface NumberColor {
    value: number
    color: string
}

(window as unknown as {
    rouletteComponent: (initialBetType?: number) => RouletteComponent;
}).rouletteComponent = rouletteComponent;